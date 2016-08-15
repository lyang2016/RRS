using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using RRS.BEL;
using RRS.BPL.AuthReviewChecker;
using RRS.DAL;

namespace RRS.BPL
{
    public class PtotPreAuthProcessor : IPreAuthProcessor
    {
        public AuthResponse ProcessPreAuth(AuthRequest request)
        {
            var response = new PtotAuthResponse();
            var dict = new Dictionary<string, string>();
            string authTypeId;
            string drgCode;

            var ptotRequest = request as PtotAuthRequest;
            if (ptotRequest == null)
                throw new ArgumentException("PtotAuthRequest Cast Error in ProcessPreAuth.");

            // validate Primary ICD9
            var vr = DiagCodeController.ValidateDiagCode(ptotRequest.PrimaryDiagnosisCode, false, ref dict);
            if (!vr.Success)
            {
                response.IsValid = false;
                response.ErrorMessage = "INVALID";
                return response;
            }

            // validate Secondary ICD9
            if (ptotRequest.SecondaryDiagnosisCode != null && !ptotRequest.SecondaryDiagnosisCode.Equals(string.Empty))
            {
                vr = DiagCodeController.ValidateDiagCode(ptotRequest.SecondaryDiagnosisCode, false, ref dict);
                if (!vr.Success)
                {
                    response.IsValid = false;
                    response.ErrorMessage = "INVALID";
                    return response;
                }
            }

            if (string.IsNullOrEmpty(ptotRequest.RealAuthTypeId))
            {
                response.IsValid = false;
                response.ErrorMessage = "Empty RealAuthTypeId entered.";
                return response;
            }

            // Step 1: get provider information based on IVR access code.
            Provider provider;
            try
            {
                provider = ProviderAdapter.GetProviderInfo(ptotRequest, ptotRequest.RealAuthTypeId);
                provider.Suffix = "PTOT";
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting provider information due to: " + ex.Message;
                return response;
            }

            // Step 1.5: get real PTOT auth type
            //vr = MemberController.ValidateMemberPtotAuthType(ptotRequest.IvrCode, ptotRequest.SubscriberId,
            //                                                 ptotRequest.MemberSeq,
            //                                                 ptotRequest.StartDate, ref dict);
            //if (vr.Success)
            //{
            //    authTypeId = dict["authtype_id"];
            //}
            //else
            //{
            //    response.IsValid = false;
            //    response.ErrorMessage = vr.Reason;
            //    return response;                
            //}

            if (string.IsNullOrEmpty(ptotRequest.RealAuthTypeId))
            {
                authTypeId = "PTOT";
            }
            else
            {
                authTypeId = ptotRequest.RealAuthTypeId;                
            }

            
            // Step 2: get member information and Unmanaged visits
            Member member;
            try
            {
                member = MemberAdapter.GetMemberInfo(ptotRequest, authTypeId);
                member.AuthTypeId = authTypeId;
                member.SelectedServiceType = ptotRequest.ServiceType;
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting member information due to: " + ex.Message;
                return response;
            }

            // get DRG code from primary diag code
            try
            {
                drgCode = DiagCodeAdapter.GetDrgCode(ptotRequest.PrimaryDiagnosisCode, provider.Suffix);
                member.DrgCode = drgCode;
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting DRG code from Primary Diagnosis Code due to: " + ex.Message;
                return response;
            }

            // get DRG code from secondary diag code if exists
            if (!string.IsNullOrEmpty(ptotRequest.SecondaryDiagnosisCode))
            {
                try
                {
                    drgCode = DiagCodeAdapter.GetDrgCode(ptotRequest.SecondaryDiagnosisCode, provider.Suffix);
                    member.SecondDrgCode = drgCode;
                }
                catch (Exception ex)
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Error getting DRG code from Secondary Diagnosis Code due to: " + ex.Message;
                    return response;
                }
            }

            // Care Registration Handling
            if (member.CareRegistrationVisits > 0 && ptotRequest.ActualVisits == 0)
            {
                try
                {
                    var pendCode = new PendCode { AuthCode = "CRA", AuthStatus = "A" };

                    if (member.SubscriberId.ToUpper().StartsWith("DEAN"))
                    {
                        var checker = new FailToPreAuthChecker();
                        var pList = new List<PendCode>();
                        vr = checker.CheckAuthReview(ptotRequest, member, provider, ref pList);
                        if (!vr.Success)
                        {
                            response.IsValid = false;
                            response.ErrorMessage = vr.Reason;
                            return response;
                        }

                        if (pList.Count > 0)
                        {
                            pendCode = pList[0];
                        }
                    }

                    GenerateCareRegistration(ptotRequest, member, provider, response, pendCode);
                }
                catch (Exception ex)
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Error generating care registration authorization due to: " + ex.Message;
                    response.AuthNumber = 0;                    
                }

                return response;
            }

            // Treat requests for all plans with unmanaged visits or care registration visits as a continuation
            if (member.UnmanagedVisits > 0 || member.CareRegistrationVisits > 0)
            {
                ptotRequest.RequestType = "2";
            }
            

            // Step 3: Calculate PTOT Score
            try
            {
                var ptotScore = CalculateAuthScore(ptotRequest);
                member.CurrentScore = ptotScore;
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error calculating PTOT score due to: " + ex.Message;
                return response;
            }

            // get the richest DRG code and use it for the auth
            try
            {
                DetermineRichestDrg(ptotRequest, member, provider);
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting the richest DRG due to: " + ex.Message;
                return response;
            }

            // Determine allowed visits and days (previous Step 5)
            try
            {
                MemberAdapter.GetAllowedVisitsAndDays(member, ptotRequest.RequestType, provider.Suffix, 0, 7, 8, 16, 17, 28);
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting allowed visits and days due to: " + ex.Message;
                return response;
            }

            try
            {
                MemberAdapter.GetLastScores(ptotRequest, member);
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting last PSFS score due to: " + ex.Message;
                return response;
            }

            member.SkipImprovementCheck = false;
            if (member.UnmanagedVisits > 0)
            {
                try
                {
                    member.SkipImprovementCheck = !MemberAdapter.FoundLastScore(ptotRequest, member);
                }
                catch (Exception ex)
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Error finding last score due to: " + ex.Message;
                    return response;
                }
            }

            // Step 4: Determine Auth Review reasons
            var pendList = new List<PendCode>();

            var checkerList = new List<IAuthReviewChecker>();
            checkerList.Add(new FailToPreAuthChecker());    // Check for Failure to Preauth (DATH)
            checkerList.Add(new RetroReviewChecker());      // Check for Retro Review (PRR) 
            checkerList.Add(new WorkInjuryChecker());       // Check to see if this request is a work related injury (WC)
            checkerList.Add(new ProviderPendChecker());     // Check for Provider Pend (PDP)
            checkerList.Add(new GroupPendChecker());        // Check for Group Pend (PDN)
            checkerList.Add(new DivisionPendChecker());     // Check for Division Pend (PDD)
            checkerList.Add(new SubscriberPendChecker());   // Check for Subscriber Pend (PDS)
            checkerList.Add(new MemberPendChecker());       // Check for Member Pend (PDM)
            checkerList.Add(new PendingAuthChecker());      // Check for the pending auths for the same patient/provider combination (PA)
            checkerList.Add(new RecentAuthChecker());       // Check for recent authorizations entered since 21 days ago (CA)
            checkerList.Add(new VisitsToDateChecker());     // Check for visits to date limit (PV & VC)
            checkerList.Add(new MemberDeniedChecker());     // Check to see if member has been denied in the past (DF)
            checkerList.Add(new PreviousAuthsChecker());    // Check previous multiple auths limit (MA)
            checkerList.Add(new FutureAuthsChecker());      // Check to see if member has any future authorization (FA)
            checkerList.Add(new DecisionTypeChecker());     // Check previous AD or MN Decision Type within the past 90 days (PP)
            if (ptotRequest.MaxVisits > 0)
            {
                checkerList.Add(new PlanMaxChecker());      // Benefit plan max reached (AU)
            }
            if (!ptotRequest.RequestType.Equals("1") && !provider.AuthWaiver && !member.SkipImprovementCheck)
            {
                checkerList.Add(new ImprovementChecker());  // Check for improvement for continuation only (EP)
            }
            checkerList.Add(new DrgChecker());              // Check for DRG code (DG)
            checkerList.Add(new PcpReferralChecker());      // Check for PCP Referral required or not (PCPR)
            if (ptotRequest.RequestType.Equals("1") && member.PlanVisitMaxInitial > 0)
            {
                checkerList.Add(new PlanMaxInitialChecker());   // Internal Threshhold check for an Inital auth (TI)
            }
            if (ptotRequest.RequestType.Equals("2") && member.PlanVisitMaxContinuation > 0)
            {
                checkerList.Add(new PlanMaxContinuationChecker());  // Internal Threshhold check for a Continuation auth (TC)
            }
            

            // Run all the checkers above to check if the auth needs review
            foreach (var checker in checkerList)
            {
                vr = checker.CheckAuthReview(ptotRequest, member, provider, ref pendList);
                if (!vr.Success)
                {
                    response.IsValid = false;
                    response.ErrorMessage = vr.Reason;
                    return response;
                }
            }

            // Get approved visits and days
            try
            {
                GetApprovedVisitsAndDays(ptotRequest, member, provider);
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting approved visits and days due to: " + ex.Message;
                return response;
            }

            // PP is the "default" Auth code if for some strange reason the auth gets through all
            // the checks, but still has no visits approved.
            if (member.ApprovedVisits < 1)
            {
                member.ApprovedVisits = 0;
                pendList.Add(new PendCode { AuthCode = "PP", AuthStatus = "RCR" });
            }

            // when no errors and visits approved are less than visits requested, set flag to "y" 
            // so web form can ask provider either accept or reject visits approved.
            var askProviderFlag = "N";
            if (pendList.Count < 1)
            {
                if (member.ApprovedVisits < ptotRequest.NumberOfVisits)
                {
                    if (member.InternalPlanMaxApplied)
                    {
                        if (request.RequestType.Equals("1"))
                        {
                            askProviderFlag = "I";
                        }
                        else
                        {
                            askProviderFlag = "C";
                        }
                    }
                    else
                    {
                        if (!member.BenefitMaxApplied)
                        {
                            askProviderFlag = "Y";
                        }
                        else
                        {
                            askProviderFlag = "B";
                        }
                    }
                }
            }

            member.VisitsRequested = ptotRequest.NumberOfVisits;
            if (!ptotRequest.SendToReview)
            {
                if (!askProviderFlag.Equals("N") && !ptotRequest.AcceptAuthReduction)
                {
                    response.IsValid = true;
                    response.ErrorMessage = string.Empty;
                    response.IsApproveReduction = true;
                    response.VisitsApproved = member.ApprovedVisits;
                    response.DaysApproved = member.ApprovedDays;
                    return response;
                }

                if (ptotRequest.AcceptAuthReduction)
                {
                    askProviderFlag = "N";
                    member.VisitsRequested = member.ApprovedVisits;       // WI #12121
                    response.IsApproveReduction = false;
                }
            }

            if (askProviderFlag.Equals("N"))
            {
                response.IsApproveReduction = false;
            }

            // index is -1 ???
            if (ptotRequest.SendToReview)
            {
                if (askProviderFlag.Equals("B"))
                    pendList.Add(new PendCode { AuthCode = "AU", AuthStatus = "RCR" });
                else if (askProviderFlag.Equals("I"))
                    pendList.Add(new PendCode { AuthCode = "TI", AuthStatus = "RCR" });
                else if (askProviderFlag.Equals("C"))
                    pendList.Add(new PendCode { AuthCode = "TC", AuthStatus = "RCR" });
                else
                    pendList.Add(new PendCode { AuthCode = "PP", AuthStatus = "RCR" });
            }

            // Check for Audit ???
            member.AuditFlag = "0";

            try
            {
                GenerateAuthorization(ptotRequest, member, pendList, provider, response);
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error generating authorization due to: " + ex.Message;
                response.AuthNumber = 0;
            }


            return response;
        }


        private void DetermineRichestDrg(PtotAuthRequest request, Member member, Provider provider)
        {
            int nextVisits;

            // get allowed visits for primary diagnosis code
            int visits = MemberAdapter.GetAllowedVisits(member.DrgCode, member.CurrentScore, request.RequestType, provider.Suffix, 0, 7, 8, 16, 17, 28);

            // get allowed visits for secondary diagnosis code if exists
            if (!string.IsNullOrEmpty(member.SecondDrgCode))
            {
                nextVisits = MemberAdapter.GetAllowedVisits(member.SecondDrgCode, member.CurrentScore, request.RequestType, provider.Suffix, 0, 7, 8, 16, 17, 28);
                if (nextVisits > visits)
                {
                    member.DrgCode = member.SecondDrgCode;
                }
            }
        }

        private int CalculateAuthScore(PtotAuthRequest request)
        {
            var rawScore = 0;

            // Condition is recurring = add 1, chronic = add 1
            if (request.Condition.Equals("2"))
                rawScore += 1;
            else if (request.Condition.Equals("3"))
                rawScore += 1;

            // Injury Type work = add 1, auto = add 1, post surgery = add 1, other = add 1
            if (request.WorkRelated)
                rawScore += 1;
            if (request.AutoAccident)
                rawScore += 1;
            if (request.PostSurgery)
                rawScore += 1;
            if (request.OtherInjury)
                rawScore += 1;

            // Duration 1-3 months = add 1, >3 months = add 2
            if (request.Duration.Equals("2"))
                rawScore += 1;
            else if (request.Duration.Equals("3"))
                rawScore += 2;

            // Body Regions 2 boxes checked = add 1, >=3 boxes checked = add 2
            var regionCount = 0;
            if (request.UpperExtremity)
                regionCount += 1;
            if (request.LowerExtremity)
                regionCount += 1;
            if (request.LsSpine)
                regionCount += 1;
            if (request.CtSpine)
                regionCount += 1;
            if (request.HandWrist)
                regionCount += 1;
            if (request.OtherRegion)
                regionCount += 1;

            if (regionCount == 2)
                rawScore += 1;
            else if (regionCount >= 3)
                rawScore += 2;

            // PSFS 0-4.5 = add 3, 4.6-7.4 = add 2, 7.5-10 = add 0
            if (request.PsfsScore >= 0m && request.PsfsScore <= 4.5m)
                rawScore += 3;
            else if (request.PsfsScore >= 4.6m && request.PsfsScore <= 7.4m)
                rawScore += 2;

            // Pain Rating 0-4 = add 0, 5-7 = add 2, 8-10 = add 3
            if (request.PainRating >= 5 && request.PainRating <= 7)
                rawScore += 2;
            else if (request.PainRating >= 8 && request.PainRating <= 10)
                rawScore += 3;

            // Pain history = add 1
            if (request.HasPainHistory)
                rawScore += 1;

            // Take Opioids = add 1
            if (request.TakeOpioids)
                rawScore += 1;

            // Overweight = add 1
            if (request.IsOverweight)
                rawScore += 1;

            // Depression = add 1
            if (request.HasDepression)
                rawScore += 1;

            // Limit Communication = add 1
            if (request.LimitCommunication)
                rawScore += 1;

            // Diabetes = add 1
            if (request.HasDiabetes)
                rawScore += 1;

            // CNS = add 1
            if (request.HasCns)
                rawScore += 1;

            // Cardiovascular = add 1
            if (request.HasCardiovascular)
                rawScore += 1;

            // Cancer = add 1
            if (request.HasCancer)
                rawScore += 1;

            // Lung Disease = add 1
            if (request.HasLungDisease)
                rawScore += 1;

            // Smoker = add 1
            if (request.IsSmoker)
                rawScore += 1;

            // Drinker = add 1
            if (request.IsDrinker)
                rawScore += 1;

            // No Exercise = add 1
            if (!request.HasExercise)
                rawScore += 1;

            return rawScore;
        }

        private void GetApprovedVisitsAndDays(PtotAuthRequest request, Member member, Provider provider)
        {
            int autoApprovedVisits;
            CompanyData companyData;

            try
            {
                autoApprovedVisits = MemberAdapter.GetAutoApprovedVisits(member);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting auto approved visits due to: " + ex.Message);
            }

            try
            {
                companyData = CompanyDataAdapter.GetCompanyData();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting company data due to: " + ex.Message);
            }

            if (request.RequestType.Equals("1"))    // New Auth        
            {
                if (request.NumberOfWeeks * 7 < member.AllowedDays)
                {
                    member.ApprovedDays = request.NumberOfWeeks * 7;
                }
                else
                {
                    member.ApprovedDays = member.AllowedDays;
                }

                if (request.NumberOfVisits <= member.AllowedVisits)
                {
                    member.ApprovedVisits = request.NumberOfVisits;
                }
                else
                {
                    member.ApprovedVisits = member.AllowedVisits;
                }

                if (autoApprovedVisits > member.AllowedVisits)
                {
                    if (request.NumberOfVisits <= autoApprovedVisits)
                    {
                        member.ApprovedVisits = request.NumberOfVisits;
                    }
                    else
                    {
                        member.ApprovedVisits = autoApprovedVisits;
                    }
                }
            }
            else    // Continuation of care
            {
                if (provider.AuthWaiver)
                {
                    if (request.NumberOfVisits <= companyData.WaiverVisits)
                    {
                        member.ApprovedVisits = request.NumberOfVisits;
                    }
                    else
                    {
                        member.ApprovedVisits = companyData.WaiverVisits;
                    }

                    if (request.NumberOfWeeks * 7 <= companyData.WaiverDays)
                    {
                        member.ApprovedDays = request.NumberOfWeeks * 7;
                    }
                    else
                    {
                        member.ApprovedDays = companyData.WaiverDays;
                    }
                }
                else
                {
                    if (request.NumberOfVisits <= member.AllowedVisits)
                    {
                        member.ApprovedVisits = request.NumberOfVisits;
                    }
                    else
                    {
                        member.ApprovedVisits = member.AllowedVisits;
                    }

                    if (request.NumberOfWeeks * 7 <= member.AllowedDays)
                    {
                        member.ApprovedDays = request.NumberOfWeeks * 7;
                    }
                    else
                    {
                        member.ApprovedDays = member.AllowedDays;
                    }
                }
            }

            // new code to ensure that visits approved are not greater than plan max.
            // only goes into this code when the benefit plan has maximum visits defined.
            if (request.RequestType.Equals("1"))
            {
                if (member.PlanVisitMaxInitial > 0 && request.MaxVisits > 0)
                {
                    if (member.PlanVisitMaxInitial < request.MaxVisits)
                    {
                        CheckPlanMaxInitial(request, member);
                    }
                    else
                    {
                        CheckPlanMax(request, member);
                    }
                }
                else if (member.PlanVisitMaxInitial > 0 && request.MaxVisits == 0)
                {
                    CheckPlanMaxInitial(request, member);
                }
                else if (member.PlanVisitMaxInitial == 0 && request.MaxVisits > 0)
                {
                    CheckPlanMax(request, member);
                }
            }
            else
            {
                if (member.PlanVisitMaxContinuation > 0 && request.MaxVisits > 0)
                {
                    if (member.PlanVisitMaxContinuation < request.MaxVisits)
                    {
                        CheckPlanMaxContinuation(request, member);
                    }
                    else
                    {
                        CheckPlanMax(request, member);
                    }
                }
                else if (member.PlanVisitMaxContinuation > 0 && request.MaxVisits == 0)
                {
                    CheckPlanMaxContinuation(request, member);
                }
                else if (member.PlanVisitMaxContinuation == 0 && request.MaxVisits > 0)
                {
                    CheckPlanMax(request, member);
                }
            }
        }

        private void CheckPlanMax(PtotAuthRequest request, Member member)
        {
            if (request.ActualVisits < request.MaxVisits)
            {
                var remainingVisits = request.MaxVisits - request.ActualVisits;
                if (remainingVisits < member.ApprovedVisits)
                {
                    member.ApprovedVisits = remainingVisits;
                    member.BenefitMaxApplied = true;
                }
            }
        }

        private void CheckPlanMaxInitial(PtotAuthRequest request, Member member)
        {
            if (request.ActualVisits < member.PlanVisitMaxInitial)
            {
                var remainingVisits = member.PlanVisitMaxInitial - request.ActualVisits;
                if (remainingVisits < member.ApprovedVisits)
                {
                    member.ApprovedVisits = remainingVisits;
                    member.InternalPlanMaxApplied = true;
                }
            }
        }

        private void CheckPlanMaxContinuation(PtotAuthRequest request, Member member)
        {
            if (request.ActualVisits < member.PlanVisitMaxContinuation)
            {
                var remainingVisits = member.PlanVisitMaxContinuation - request.ActualVisits;
                if (remainingVisits < member.ApprovedVisits)
                {
                    member.ApprovedVisits = remainingVisits;
                    member.InternalPlanMaxApplied = true;
                }
            }
        }



        private void GenerateAuthorization(PtotAuthRequest request, Member member, List<PendCode> pendList,
                                Provider provider, PtotAuthResponse response)
        {
            var notes = "Pain = " + request.PainRating +
                        //", New or Recurring = " + (request.Condition.Equals("1") ? 0 : 1) + "," + Environment.NewLine + 
                        ", Condition = " + request.Condition + "," + Environment.NewLine + 
                        "Has PCP Referral = " + (request.HasReferral ? 1 : 0) +
                        ", Score = " + member.CurrentScore + ".";

            if (request.PsfsScore == -1m)
            {
                notes += " PSFS Score = ." + Environment.NewLine;
            }
            else
            {
                notes += " PSFS Score = " + request.PsfsScore + "." + Environment.NewLine;
            }

            notes += "Entered via: " + Environment.MachineName + ". " + "Treatment within past 6 months = " + request.PriorTreatment + 
                    ", Visits Requested = " + request.NumberOfVisits + "." + Environment.NewLine +
                    "Benefit year: Type = " + request.VisitYearType +
                    ", From = " + request.ActualVisitStartDate.ToShortDateString() +
                    " to " + request.ActualVisitEndDate.ToShortDateString() +
                    ", Previously used = " + request.ActualVisits +
                    ", Max = " + request.MaxVisits + "." + Environment.NewLine;

            member.NewOrExtended = request.RequestType.Equals("1") ? 1 : 0;

            if (pendList.Count < 1)
            {
                response.VisitsApproved = member.ApprovedVisits;
                response.DaysApproved = member.ApprovedDays;
                member.AdditionalVisits = member.ApprovedVisits;
                response.DateApprovedFrom = request.StartDate;
                response.DateApprovedThru = request.StartDate.AddDays(member.ApprovedDays);

                // BCBSMA can't be approved past the end of the year of the start date, 
                // also for CignaWA CalYear plans
                // Add CignaWAUnlimited plan (WI 13479)
                if (member.GroupId.Contains("BCBSMA_UM") || Regex.IsMatch(member.PlanId, "CignaWA[a-zA-Z0-9_]*CalYear") ||
                    member.PlanId.Trim().Equals("CignaWAUnlimited", StringComparison.OrdinalIgnoreCase))
                {
                    var endOfStartDateYear = new DateTime(request.StartDate.Year, 12, 31);
                    if (response.DateApprovedThru > endOfStartDateYear)
                        response.DateApprovedThru = endOfStartDateYear;
                }

                // Highmark and Dean can't be approved past the end of the year of the start date too
                if (request.SubscriberId.ToUpper().StartsWith("HIGH") || request.SubscriberId.ToUpper().StartsWith("DEAN"))
                {
                    var endOfStartDateYear = new DateTime(request.StartDate.Year, 12, 31);
                    if (response.DateApprovedThru > endOfStartDateYear)
                        response.DateApprovedThru = endOfStartDateYear;
                }

                member.ApporvedBy = "IVR";
                response.AuthCode = "IVR";
                response.AuthStatus = "A";
                member.DenialLetterPrinted = "1";
                member.DenialLetterDate = DateTime.Now;
                member.DenialLetterUser = "IVR";
            }
            else
            {
                response.AuthCode = pendList[0].AuthCode;
                response.AuthStatus = pendList[0].AuthStatus;
                response.VisitsApproved = 0;
                response.DaysApproved = 0;
                member.AdditionalVisits = 0;
                response.DateApprovedFrom = new DateTime(1900, 1, 1);
                response.DateApprovedThru = new DateTime(1900, 1, 1);

                member.ApporvedBy = string.Empty;
                member.DenialLetterPrinted = "0";
                member.DenialLetterDate = new DateTime(1900, 1, 1);
                member.DenialLetterUser = string.Empty;

                var additionalCodes = string.Empty;
                if (pendList.Count > 1)
                {
                    for (var i = 1; i < pendList.Count; i++)
                    {
                        additionalCodes += pendList[i].AuthCode + ", ";
                    }
                }
                else
                {
                    additionalCodes = "None";
                }

                notes += "Additional Error Codes = " + additionalCodes + "." + Environment.NewLine;
            }

            if (response.VisitsApproved < 1 && response.AuthStatus.Equals("A"))
            {
                response.AuthCode = "PP";
                response.AuthStatus = "RCRP";
            }


            /*
             * Prevent Highmark to postback existing ClientAuthNumber with the same memberId and providerId 
             */
            if (!DbHelper.NullString(request.ClientAuthNumber).Equals(string.Empty) && request.SubscriberId.StartsWith("HIGH")
                && request.IsNaviNet)
            {
                try
                {
                    if (AuthAdapter.CheckDuplicateClientAuthNumber(request.ClientAuthNumber, request.SubscriberId, provider.Id))
                    {
                        response.IsValid = false;
                        response.ErrorMessage = "Duplicate ClientAuthNumber exists in the previous auth";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Error checking duplicate ClientAuthNumber due to: " + ex.Message;
                    return;
                }
            }


            /*
                Walter 20100112      
                We are going to prevent auths when there is an existing auth in the last five minutes.    
                We've had issues with IIS postbacks that have caused duplicate auths so we are addressing that.    
            */
            try
            {
                if (AuthAdapter.CheckDuplicateAuth(member, provider, request.StartDate))
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Previous auth exists in five minutes";
                    return;
                }
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error checking duplicate auth due to: " + ex.Message;
                return;
            }


            try
            {
                var dict = AuthAdapter.GetCaseNumber(request, member, provider);
                member.CaseNumber = dict["case_num"];
                // Exclude all plans with Unmanaged Visits from any changes to request type
                if (dict["new_notes"] != string.Empty && member.UnmanagedVisits == 0)
                {
                    notes += dict["new_notes"];
                    member.NewOrExtended = dict["new_or_extended"].Equals("1") ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting case number due to: " + ex.Message;
                return;
            }

            member.AuthNotes = notes;


            using (var ts = new TransactionScope())
            {
                response.AuthNumber = AuthAdapter.GetNextAuthNumber(1);

                if (DbHelper.NullString(request.ClientAuthNumber).Equals(string.Empty) && request.SubscriberId.StartsWith("HIGH")
                    && !request.IsNaviNet)
                {
                    request.ClientAuthNumber = "H" + DbHelper.PadWithLeadingZero(response.AuthNumber.ToString(CultureInfo.InvariantCulture), 9);
                }

                AuthAdapter.InsertAuthForPtot(request, member, provider, response);

                // Only Dean and Highmark will have PT/OT
                // Get bcbsma_number and update auth table accordingly
                //if (member.GroupId.Contains("BCBSMA_UM") && response.AuthStatus.Equals("A"))
                //{
                //    response.BcbsmaNumber = AuthAdapter.GetBcbsmaNumber(response.AuthNumber);
                //}
                //else
                //{
                //    response.BcbsmaNumber = string.Empty;
                //}

                if (member.SubscriberId.ToUpper().StartsWith("HIGH") && member.GroupId.ToUpper().StartsWith("HM"))
                {
                    AuthAdapter.ModifyPreviousEndDate(response.AuthNumber);
                }

                AuthAdapter.InsertAuthDiagCodes(response.AuthNumber, request, member.AuthTypeId);
                AuthAdapter.InsertAuthLogForPtot(request, member, response);

                if (pendList.Any(item => item.AuthCode.Equals("PCPR")) && response.AuthCode != "PCPR")
                {
                    AuthAdapter.InsertAuthEvent(response.AuthNumber, member.CaseNumber);
                }

                ts.Complete();
            }

            response.IsValid = true;
            response.ErrorMessage = string.Empty;
            response.DateApproved = DateTime.Now;
            response.IsApproveReduction = false;
            response.ClientAuthNumber = DbHelper.NullString(request.ClientAuthNumber);
            response.FailToPreAuthDays = member.FailToPreAuthDays;
        }

        private void GenerateCareRegistration(PtotAuthRequest request, Member member, Provider provider, AuthResponse response, PendCode pendCode)
        {
            if (pendCode.AuthCode == "CRA")
            {
                response.VisitsApproved = member.CareRegistrationVisits;
                response.DateApprovedFrom = request.StartDate;
                response.DateApprovedThru = request.ActualVisitEndDate;

                member.ApporvedBy = "IVR";
                response.AuthCode = "CRA";
                response.AuthStatus = "A";                
            }
            else if (pendCode.AuthCode == "DATH")
            {
                response.VisitsApproved = 0;
                response.DateApprovedFrom = new DateTime(1900, 1, 1);
                response.DateApprovedThru = new DateTime(1900, 1, 1);

                member.ApporvedBy = string.Empty;
                response.AuthCode = "DATH";
                response.AuthStatus = "D";
            }
            else
            {
                return;
            }

            /*
             * Prevent Highmark to postback existing ClientAuthNumber with the same memberId and providerId 
             */
            if (!DbHelper.NullString(request.ClientAuthNumber).Equals(string.Empty) && request.SubscriberId.StartsWith("HIGH")
                && request.IsNaviNet)
            {
                try
                {
                    if (AuthAdapter.CheckDuplicateClientAuthNumber(request.ClientAuthNumber, request.SubscriberId, provider.Id))
                    {
                        response.IsValid = false;
                        response.ErrorMessage = "Duplicate ClientAuthNumber exists in the previous auth";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Error checking duplicate ClientAuthNumber due to: " + ex.Message;
                    return;
                }
            }


            /*
                Walter 20100112      
                We are going to prevent auths when there is an existing auth in the last five minutes.    
                We've had issues with IIS postbacks that have caused duplicate auths so we are addressing that.    
            */
            try
            {
                if (AuthAdapter.CheckDuplicateAuth(member, provider, request.StartDate))
                {
                    response.IsValid = false;
                    response.ErrorMessage = "Previous auth exists in five minutes";
                    return;
                }
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error checking duplicate auth due to: " + ex.Message;
                return;
            }

            try
            {
                if (MemberAdapter.IsMedicareAdvantageMember(member))
                {
                    member.IsMedicareAdvantageMember = true;
                }
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error checking Medicare Advantage member due to: " + ex.Message;
                return;
            }

            try
            {
                var dict = AuthAdapter.GetCaseNumber(request, member, provider);
                member.CaseNumber = dict["case_num"];
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.ErrorMessage = "Error getting case number due to: " + ex.Message;
                return;
            }


            using (var ts = new TransactionScope())
            {
                response.AuthNumber = AuthAdapter.GetNextAuthNumber(1);

                if (DbHelper.NullString(request.ClientAuthNumber).Equals(string.Empty) && request.SubscriberId.StartsWith("HIGH")
                    && !request.IsNaviNet)
                {
                    request.ClientAuthNumber = "H" + DbHelper.PadWithLeadingZero(response.AuthNumber.ToString(CultureInfo.InvariantCulture), 9);
                }

                AuthAdapter.InsertCareRegistration(request, member, provider, response, pendCode);

                AuthAdapter.InsertAuthDiagCodes(response.AuthNumber, request, member.AuthTypeId);
                AuthAdapter.InsertCareRegistrationLog(request, member, response);

                ts.Complete();
            }

            response.IsValid = true;
            response.ErrorMessage = string.Empty;
            response.IsApproveReduction = false;
            response.ClientAuthNumber = DbHelper.NullString(request.ClientAuthNumber);

            if (pendCode.AuthCode == "CRA")
            {
                response.DateApproved = DateTime.Now;
            }
            else
            {
                response.DateApproved = new DateTime(1900, 1, 1);
            }
        }
    }
}
