using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using RRS.BEL;
using RRS.DAL;
using RRS.BPL;
using NLog;

namespace RRS.Services
{
    public class PreAuthManager : IPreAuthManager
    {
        private static readonly Logger Logger = LogManager.GetLogger("PreAuthManager");

        public string HelloWorld()
        {
            Logger.Info("HelloWorld is called.");

            string result;

            //var userName = System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.Name;

            //if (string.IsNullOrEmpty(userName))
            //{
            //    userName = string.Empty;
            //}

            //result = "Welcome " + userName;

            result = "Hello World";
            return result;
        }

        public ChiroAuthResponse GetChiroPreAuth(ChiroAuthRequest request)
        {
            var processor = new ChiroPreAuthProcessor();
            var controller = new PreAuthController(processor, request);
            var response = controller.ProcessPreAuth();
            var chiroResponse = response as ChiroAuthResponse;

            if (chiroResponse != null && !chiroResponse.IsValid)
            {
                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Chiro Pre Auth for IvrCode={0}, SubscriberId={1}, MemberSeq={2} has the following error: {3}", request.IvrCode, request.SubscriberId, request.MemberSeq, response.ErrorMessage);
                Logger.Info("The original submitted data for the above Chiro Pre Auth request are listed below:");
                Logger.Info("Ivr Code={0}", request.IvrCode);
                Logger.Info("Subscriber Id={0}", request.SubscriberId);
                Logger.Info("Member Seq={0}", request.MemberSeq);
                Logger.Info("Start Date={0}", request.StartDate.ToShortDateString());
                Logger.Info("Request Type={0}", DbHelper.NullString(request.RequestType));
                Logger.Info("Condition={0}", DbHelper.NullString(request.Condition));
                //Logger.Info("Injury Type={0}", DbHelper.NullString(request.InjuryType));
                Logger.Info("Work Related={0}", request.WorkRelated);
                Logger.Info("Auto Accident={0}", request.AutoAccident);
                Logger.Info("Other Injury={0}", request.OtherInjury);
                Logger.Info("Post Surgery={0}", request.PostSurgery);
                Logger.Info("No Injury={0}", request.NoInjury);
                //Logger.Info("Initial Injury Date={0}", DbHelper.NullDateTime(request.InitialInjuryDate).ToShortDateString());
                Logger.Info("Duration={0}", DbHelper.NullString(request.Duration));
                Logger.Info("Initial Visit Date={0}", DbHelper.NullDateTime(request.InitialVisitDate).ToShortDateString());
                Logger.Info("Visits To Date={0}", request.VisitsToDate);
                Logger.Info("Visits Requested={0}", request.NumberOfVisits);
                Logger.Info("Weeks Requested={0}", request.NumberOfWeeks);
                Logger.Info("Primary Diagnosis Code={0}", request.PrimaryDiagnosisCode);
                Logger.Info("Secondary Diagnosis Code={0}", DbHelper.NullString(request.SecondaryDiagnosisCode));
                Logger.Info("Third Diagnosis Code={0}", DbHelper.NullString(request.ThirdDiagnosisCode));
                //Logger.Info("Daily Living Rating={0}", request.DailyLivingRating);
                Logger.Info("Pain Rating={0}", request.PainRating);
                //Logger.Info("Range of Motion Rating={0}", request.RangeOfMotionRating);
                Logger.Info("Limit Communication={0}", request.LimitCommunication);
                Logger.Info("Has Exercise={0}", request.HasExercise);
                Logger.Info("Co-Treating={0}", request.IsCoTreat);
                //Logger.Info("FRI Score={0}", request.FriScore);
                Logger.Info("PSFS Score={0}", request.PsfsScore);
                Logger.Info("Has Diabetes={0}", request.HasDiabetes);
                Logger.Info("Has Stroke={0}", request.HasStroke);
                Logger.Info("Has Cancer={0}", request.HasCancer);
                Logger.Info("Is Smoker={0}", request.IsSmoker);
                Logger.Info("Is Overweight={0}", request.IsOverweight);
                //Logger.Info("Has Chronic Pain={0}", request.HasChronicPain);
                Logger.Info("Has Pain History={0}", request.HasPainHistory);
                Logger.Info("Has Depression={0}", request.HasDepression);
                Logger.Info("Prior Auth Number={0}", DbHelper.NullString(request.PriorAuthNumber));
                Logger.Info("Max Visits={0}", request.MaxVisits);
                Logger.Info("Actual Visits={0}", request.ActualVisits);
                Logger.Info("Actual Visit Start Date={0}", DbHelper.NullDateTime(request.ActualVisitStartDate).ToShortDateString());
                Logger.Info("Actual Visit End Date={0}", DbHelper.NullDateTime(request.ActualVisitEndDate).ToShortDateString());
                Logger.Info("Visit Year Type={0}", DbHelper.NullString(request.VisitYearType));
                Logger.Info("Accept Auth Reduction={0}", request.AcceptAuthReduction);
                Logger.Info("Send To Review={0}", request.SendToReview);
                Logger.Info("Has Referral={0}", request.HasReferral);
                Logger.Info("Application Id={0}", request.ApplicationId);
                Logger.Info("Is NaviNet={0}", request.IsNaviNet);
                Logger.Info("Client Auth Number={0}", DbHelper.NullString(request.ClientAuthNumber));
            }

            return chiroResponse;
        }


        public PtotAuthResponse GetPtotPreAuth(PtotAuthRequest request)
        {
            var processor = new PtotPreAuthProcessor();
            var controller = new PreAuthController(processor, request);
            var response = controller.ProcessPreAuth();
            var ptotResponse = response as PtotAuthResponse;

            if (ptotResponse != null && !ptotResponse.IsValid)
            {
                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("PTOT Pre Auth for IvrCode={0}, SubscriberId={1}, MemberSeq={2} has the following error: {3}", request.IvrCode, request.SubscriberId, request.MemberSeq, response.ErrorMessage);
                Logger.Info("The original submitted data for the above PTOT Pre Auth request are listed below:");
                Logger.Info("Ivr Code={0}", request.IvrCode);
                Logger.Info("Subscriber Id={0}", request.SubscriberId);
                Logger.Info("Member Seq={0}", request.MemberSeq);
                Logger.Info("Has Referral={0}", request.HasReferral);
                Logger.Info("Start Date={0}", request.StartDate.ToShortDateString());
                Logger.Info("Request Type={0}", DbHelper.NullString(request.RequestType));
                Logger.Info("Condition={0}", DbHelper.NullString(request.Condition));
                Logger.Info("Work Related={0}", request.WorkRelated);
                Logger.Info("Auto Accident={0}", request.AutoAccident);
                Logger.Info("Other Injury={0}", request.OtherInjury);
                Logger.Info("Post Surgery={0}", request.PostSurgery);
                Logger.Info("No Injury={0}", request.NoInjury);
                Logger.Info("Duration={0}", DbHelper.NullString(request.Duration));
                Logger.Info("Initial Visit Date={0}", DbHelper.NullDateTime(request.InitialVisitDate).ToShortDateString());
                Logger.Info("Prior Treatment={0}", DbHelper.NullString(request.PriorTreatment));
                Logger.Info("Visits Requested={0}", request.NumberOfVisits);
                Logger.Info("Weeks Requested={0}", request.NumberOfWeeks);
                Logger.Info("Primary Diagnosis Code={0}", request.PrimaryDiagnosisCode);
                Logger.Info("Secondary Diagnosis Code={0}", DbHelper.NullString(request.SecondaryDiagnosisCode));
                Logger.Info("UE={0}", request.UpperExtremity);
                Logger.Info("LE={0}", request.LowerExtremity);
                Logger.Info("L/S Spine={0}", request.LsSpine);
                Logger.Info("C/T Spine={0}", request.CtSpine);
                Logger.Info("Hand/Wrist={0}", request.HandWrist);
                Logger.Info("Other Region={0}", request.OtherRegion);
                Logger.Info("PSFS Score={0}", request.PsfsScore);
                Logger.Info("Pain Rating={0}", request.PainRating);
                Logger.Info("Has Pain History={0}", request.HasPainHistory);
                Logger.Info("Is Smoker={0}", request.IsSmoker);
                Logger.Info("Is Drinker={0}", request.IsDrinker);
                Logger.Info("Take Opioids={0}", request.TakeOpioids);
                Logger.Info("Is Overweight={0}", request.IsOverweight);
                Logger.Info("Has Exercise={0}", request.HasExercise);
                Logger.Info("Has Confidence={0}", request.HasConfidence);
                Logger.Info("Has Depression={0}", request.HasDepression);
                Logger.Info("Limit Communication={0}", request.LimitCommunication);
                Logger.Info("Has Diabetes={0}", request.HasDiabetes);
                Logger.Info("Has CNS={0}", request.HasCns);
                Logger.Info("Has Cardiovascular Condition={0}", request.HasCardiovascular);
                Logger.Info("Has Cancer={0}", request.HasCancer);
                Logger.Info("Has Lung Disease={0}", request.HasLungDisease);
                Logger.Info("Max Visits={0}", request.MaxVisits);
                Logger.Info("Actual Visits={0}", request.ActualVisits);
                Logger.Info("Actual Visit Start Date={0}", DbHelper.NullDateTime(request.ActualVisitStartDate).ToShortDateString());
                Logger.Info("Actual Visit End Date={0}", DbHelper.NullDateTime(request.ActualVisitEndDate).ToShortDateString());
                Logger.Info("Visit Year Type={0}", DbHelper.NullString(request.VisitYearType));
                Logger.Info("Accept Auth Reduction={0}", request.AcceptAuthReduction);
                Logger.Info("Send To Review={0}", request.SendToReview);
                Logger.Info("Application Id={0}", request.ApplicationId);
                Logger.Info("Is NaviNet={0}", request.IsNaviNet);
                Logger.Info("Client Auth Number={0}", DbHelper.NullString(request.ClientAuthNumber));
                Logger.Info("Real AuthType={0}", DbHelper.NullString(request.RealAuthTypeId));
            }

            return ptotResponse;
        }


        public DataSet GetRrsQuestions(string questionSet)
        {
            return QuestionsAdapter.GetQuestions(questionSet);
        }


        public bool InsertRrsEntryLog(string callStarted, string sessionId, string phoneNumber, string variableName, 
                                    string entryString, string validationResult, string isBcbsma)
        {
            return EntryLogAdapter.InsertEntryLog(callStarted, sessionId, phoneNumber, variableName, entryString,
                                                  validationResult, isBcbsma);
        }


        public bool InsertAuthSsoLog(int authNumber, AuthSsoData ssoData)
        {
            return AuthAdapter.InsertAuthSsoLog(authNumber, ssoData);
        }


        public bool UpdateProviderFax(string providerId, string locationSeq, string fax)
        {
            return ProviderAdapter.UpdateFax(providerId, locationSeq, fax);
        }


        public DiagCodeResponse CheckDiagCode(string code, bool isIvr)
        {
            var dict = new Dictionary<string, string>();
            var vr = DiagCodeController.ValidateDiagCode(code, isIvr, ref dict);
            DiagCodeResponse cr;

            if (vr.Success)
            {
                cr = new DiagCodeResponse
                         {
                             IsValid = true,
                             ErrorMessage = string.Empty,
                             DiagCode = dict["diag_code"]
                         };
            }
            else
            {
                cr = new DiagCodeResponse
                         {
                             IsValid = false,
                             ErrorMessage = vr.Reason,
                             DiagCode = "0"
                         };
            }

            return cr;
        }

        public ProviderResponse CheckProviderFromSso(string clientPrefix, string id, DateTime requestedStartDate)
        {
            var dict = new Dictionary<string, string>();
            var vr = ProviderController.ValidateProviderFromSso(clientPrefix, id, requestedStartDate, ref dict);
            ProviderResponse pr;

            if (vr.Success)
            {
                pr = new ProviderResponse
                         {
                             IsValid = true,
                             ErrorMessage = string.Empty,
                             IvrCode = dict["ivr_code"],
                             ProviderId = clientPrefix + id,
                             AuthTypeId = dict["authtype_id"]
                         };
            }
            else
            {
                pr = new ProviderResponse
                         {
                             IsValid = false,
                             ErrorMessage = vr.Reason
                         };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Check Provider From SSO for ProviderId={0} has the following error: {1}", id, vr.Reason);
                Logger.Info("The original submitted data for the above Check Provider From SSO are listed below:");
                Logger.Info("Client Prefix={0}", clientPrefix);
                Logger.Info("Provider Id={0}", id);
            }

            return pr;
        }

        public AngelProviderResponse CheckProviderFromAngel(string ivrCode)
        {
            var dict = new Dictionary<string, string>();
            var vr = ProviderController.AngelValidateProvider(ivrCode, ref dict);
            AngelProviderResponse apr;

            if (vr.Success)
            {
                apr = new AngelProviderResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    ProviderId = dict["Provider_ID"],
                    LocationSeq = dict["Location_Seq"],
                    ZipCode = dict["Zip"],
                    TinLast4 = dict["TIN"],
                    Fax = dict["Fax_Number"],
                    IsBcbsma = bool.Parse(dict["isBCBSMA"]),
                    PlayBcbsmaIntro = bool.Parse(dict["PlayBCBSMAIntro"]),
                    PlayExcludeAlphaPrefix = bool.Parse(dict["PlayExcludeAlphaPrefix"]),
                    IvrRight1 = bool.Parse(dict["ivr_right_1"]),
                    IvrRight2 = bool.Parse(dict["ivr_right_2"]),
                    IvrRight3 = bool.Parse(dict["ivr_right_3"]),
                    IvrRight4 = bool.Parse(dict["ivr_right_4"]),
                    IvrRight5 = bool.Parse(dict["ivr_right_5"]),
                    IvrRight6 = bool.Parse(dict["ivr_right_6"]),
                    IvrRight7 = bool.Parse(dict["ivr_right_7"]),
                    NpiLast4 = dict["NPI"],
                    IsHighmark = bool.Parse(dict["isHighmark"]),
                    IsDean = bool.Parse(dict["isDean"])
                };
            }
            else
            {
                apr = new AngelProviderResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Check Provider From Angel for IvrCode={0} has the following error: {1}", ivrCode, vr.Reason);
                Logger.Info("The original submitted data for the above Check Provider From Angel are listed below:");
                Logger.Info("Ivr Code={0}", ivrCode);
            }

            return apr;
        }

        public ProviderResponse CheckProvider(string ivrCode, bool isIvr, string userId, string password)
        {
            var dict = new Dictionary<string, string>();
            var vr = ProviderController.ValidateProvider(ivrCode, isIvr, userId, password, ref dict);
            ProviderResponse pr;

            if (vr.Success)
            {
                if (isIvr)
                {
                    pr = new ProviderResponse
                             {
                                 IsValid = true,
                                 ErrorMessage = string.Empty,
                                 ProviderId = dict["Provider_ID"],
                                 LocationSeq = dict["Location_Seq"],
                                 ZipCode = dict["Zip"],
                                 TinLast4 = dict["TIN"],
                                 Fax = dict["Fax_Number"],
                                 IsBcbsma = dict["isBCBSMA"].ToUpper() == "TRUE",
                                 PlayBcbsmaIntro = dict["PlayBCBSMAIntro"].ToUpper() == "TRUE",
                                 PlayExcludeAlphaPrefix = dict["PlayExcludeAlphaPrefix"].ToUpper() == "TRUE",
                                 IvrRight1 = dict["ivr_right_1"].ToUpper() == "TRUE",
                                 IvrRight2 = dict["ivr_right_2"].ToUpper() == "TRUE",
                                 IvrRight3 = dict["ivr_right_3"].ToUpper() == "TRUE",
                                 IvrRight4 = dict["ivr_right_4"].ToUpper() == "TRUE",
                                 IvrRight5 = dict["ivr_right_5"].ToUpper() == "TRUE",
                                 IvrRight6 = dict["ivr_right_6"].ToUpper() == "TRUE",
                                 IvrRight7 = dict["ivr_right_7"].ToUpper() == "TRUE",
                                 ErrorNumber = 0
                             };
                }
                else
                {
                    pr = new ProviderResponse
                             {
                                 IsValid = true,
                                 ErrorMessage = string.Empty,
                                 ProviderName = dict["provider_name"],
                                 Address = dict["address"],
                                 City = dict["city"],
                                 State = dict["state"],
                                 ZipCode = dict["Zip"],
                                 Fax = dict["Fax_Number"],
                                 IvrRight1 = dict["ivr_right_1"] != string.Empty,
                                 IvrRight2 = dict["ivr_right_2"] != string.Empty,
                                 IvrRight3 = dict["ivr_right_3"] != string.Empty,
                                 IvrRight4 = dict["ivr_right_4"] != string.Empty,
                                 IvrRight5 = dict["ivr_right_5"] != string.Empty,
                                 IvrRight6 = dict["ivr_right_6"] != string.Empty,
                                 IvrRight7 = dict["ivr_right_7"] != string.Empty
                             };
                }
            }
            else
            {
                if (isIvr)
                {
                    pr = new ProviderResponse
                             {
                                 IsValid = false,
                                 ErrorMessage = vr.Reason,
                                 ProviderId = dict["Provider_ID"],
                                 LocationSeq = dict["Location_Seq"],
                                 ZipCode = dict["Zip"],
                                 TinLast4 = dict["TIN"],
                                 Fax = dict["Fax_Number"],
                                 ErrorNumber = int.Parse(dict["err_num"])
                             };
                }
                else
                {
                    pr = new ProviderResponse
                             {
                                 IsValid = false,
                                 ErrorMessage = vr.Reason
                             };
                }
            }

            return pr;
        }


        public MemberResponse CheckMember(string ivrCode, bool isIvr, string memberId, DateTime dateOfBirth, DateTime startDate)
        {
            var dict = new Dictionary<string, string>();
            var vr = MemberController.ValidateMember(ivrCode, isIvr, memberId, dateOfBirth, startDate, ref dict);
            MemberResponse mr;

            if (vr.Success)
            {
                mr = new MemberResponse
                         {
                             IsValid = true,
                             ErrorMessage = string.Empty,
                             MemberName = dict["member_name"],
                             MaxVisits = int.Parse(dict["plan_max_visits"]),
                             ActualVisits = int.Parse(dict["actual_visits"]),
                             SubscriberId = dict["subscriber_id"],
                             MemberSeq = dict["member_seq"],
                             PlanId = dict["plan_id"],
                             ActualVisitStartDate = DateTime.Parse(dict["actual_start_date"]),
                             ActualVisitEndDate = DateTime.Parse(dict["actual_end_date"]),
                             VisitYearType = dict["visit_year_type"],
                             UnmanagedVisits = int.Parse(dict["unmanaged_visits"]),
                             ClaimsAddressError = dict["claimsAddressError"],
                             ClaimsAddressId = int.Parse(dict["claimsAddressId"]),
                             ClaimsAddress = dict["claimsAddress"],
                             DateOfBirth = DateTime.Parse(dict["dob"]),
                             ErrorNumber = 0
                         };
            }
            else
            {
                if (isIvr)
                {
                    mr = new MemberResponse
                    {
                        IsValid = false,
                        ErrorMessage = vr.Reason,
                        MemberName = dict["member_name"],
                        MaxVisits = int.Parse(dict["plan_max_visits"]),
                        ActualVisits = int.Parse(dict["actual_visits"]),
                        SubscriberId = dict["subscriber_id"],
                        MemberSeq = dict["member_seq"],
                        PlanId = dict["plan_id"],
                        ActualVisitStartDate = DateTime.Parse(dict["actual_start_date"]),
                        ActualVisitEndDate = DateTime.Parse(dict["actual_end_date"]),
                        VisitYearType = dict["visit_year_type"],
                        UnmanagedVisits = int.Parse(dict["unmanaged_visits"]),
                        ClaimsAddressError = dict["claimsAddressError"],
                        ClaimsAddressId = int.Parse(dict["claimsAddressId"]),
                        ClaimsAddress = dict["claimsAddress"],
                        DateOfBirth = DateTime.Parse(dict["dob"]),
                        ErrorNumber = int.Parse(dict["err_num"])
                    };                    
                }
                else
                {
                    mr = new MemberResponse
                    {
                        IsValid = false,
                        ErrorMessage = vr.Reason
                    };                    
                }
            }

            return mr;
        }

        public MemberResponse CheckMemberFromSso(string clientPrefix, string subscriberId, string blindKey)
        {
            var dict = new Dictionary<string, string>();
            var vr = MemberController.ValidateMemberFromSso(clientPrefix, subscriberId, blindKey, ref dict);
            MemberResponse mr;

            if (vr.Success)
            {
                mr = new MemberResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    MemberSeq = dict["member_seq"],
                };
            }
            else
            {
                mr = new MemberResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Check Member From SSO for SubscriberId={0}, BlindKey={1} has the following error: {2}", subscriberId, blindKey, vr.Reason);
                Logger.Info("The original submitted data for the above Check Member From SSO are listed below:");
                Logger.Info("Client Prefix={0}", clientPrefix);
                Logger.Info("Subscriber Id={0}", subscriberId);
                Logger.Info("Blind Key={0}", blindKey);
            }

            return mr;
        }

        public MemberResponse CheckMemberFromWeb(string ivrCode, string subscriberId, string memberSeq, string authTypeId, DateTime dateOfBirth, DateTime startDate)
        {
            var dict = new Dictionary<string, string>();
            var vr = MemberController.ValidateMemberFromWeb(ivrCode, subscriberId, memberSeq, authTypeId, 
                                            dateOfBirth, startDate, ref dict);
            MemberResponse mr;

            if (vr.Success)
            {
                mr = new MemberResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    MemberName = dict["member_name"],
                    MaxVisits = int.Parse(dict["plan_max_visits"]),
                    ActualVisits = int.Parse(dict["actual_visits"]),
                    PlanId = dict["plan_id"],
                    ActualVisitStartDate = DateTime.Parse(dict["actual_start_date"]),
                    ActualVisitEndDate = DateTime.Parse(dict["actual_end_date"]),
                    VisitYearType = dict["visit_year_type"],
                    UnmanagedVisits = int.Parse(dict["unmanaged_visits"]),
                    ClaimsAddressError = dict["claimsAddressError"],
                    ClaimsAddressId = int.Parse(dict["claimsAddressId"]),
                    ClaimsAddress = dict["claimsAddress"],
                    DateOfBirth = DateTime.Parse(dict["dob"]),
                    IsMedicareAdvantage = dict["isMedicareAdvantage"].ToUpper() == "TRUE",
                    PrefixSubscriberId = dict["prefix_subscriber_id"],
                    UseCareRegistration = dict["useCareRegistration"].ToUpper() == "TRUE",
                    RealAuthTypeId = dict["realAuthTypeId"]
                };
            }
            else
            {
                mr = new MemberResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Check Member From Web for IvrCode={0}, SubscriberId={1}, MemberSeq={2} has the following error: {3}", ivrCode, subscriberId, memberSeq, vr.Reason);
                Logger.Info("The original submitted data for the above Check Member From Web are listed below:");
                Logger.Info("Ivr Code={0}", ivrCode);
                Logger.Info("Subscriber Id={0}", subscriberId);
                Logger.Info("Member Seq={0}", memberSeq);
                Logger.Info("AuthType Id={0}", authTypeId);
                Logger.Info("Date Of Birth={0}", dateOfBirth.ToShortDateString());
                Logger.Info("Start Date={0}", startDate.ToShortDateString());
            }

            return mr;
        }

        public DataSet CheckMemberFromAngel(string ivrCode, string subscriberId, DateTime startDate)
        {
            return MemberAdapter.CheckInsuranceId(ivrCode, subscriberId, startDate);
        }

        public MemberResponse PreCheckMemberInfo(string ivrCode, string subscriberId, string memberSeq, DateTime startDate, DateTime dateOfBirth)
        {
            var dict = new Dictionary<string, string>();
            var vr = MemberController.PreCheckMemberInfo(ivrCode, subscriberId, memberSeq, startDate, dateOfBirth, ref dict);
            MemberResponse mr;

            if (vr.Success)
            {
                mr = new MemberResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    PrefixSubscriberId = dict["prefix_subscriber_id"]
                };
            }
            else
            {
                mr = new MemberResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Pre Check Member Info for IvrCode={0}, SubscriberId={1}, MemberSeq={2} has the following error: {3}", ivrCode, subscriberId, memberSeq, vr.Reason);
                Logger.Info("The original submitted data for the above Pre Check Member Info are listed below:");
                Logger.Info("Ivr Code={0}", ivrCode);
                Logger.Info("Subscriber Id={0}", subscriberId);
                Logger.Info("Member Seq={0}", memberSeq);
                Logger.Info("Start Date={0}", startDate.ToShortDateString());
                Logger.Info("Date Of Birth={0}", dateOfBirth.ToShortDateString());
            }

            return mr;
        }

        public MemberResponse GetAuthTypeFromSelectedServiceType(string prefixSubscriberId, string memberSeq, DateTime startDate, string serviceType)
        {
            var dict = new Dictionary<string, string>();
            var vr = MemberController.GetAuthTypeFromSelectedServiceType(prefixSubscriberId, memberSeq, startDate, serviceType, ref dict);
            MemberResponse mr;

            if (vr.Success)
            {
                mr = new MemberResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    RealAuthTypeId = dict["auth_type"]
                };
            }
            else
            {
                mr = new MemberResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Get Auth Type From Selected Service for Service={0}, SubscriberId={1}, MemberSeq={2} has the following error: {3}", serviceType, prefixSubscriberId, memberSeq, vr.Reason);
                Logger.Info("The original submitted data for the above Get Auth Type From Selected Service are listed below:");
                Logger.Info("Subscriber Id={0}", prefixSubscriberId);
                Logger.Info("Member Seq={0}", memberSeq);
                Logger.Info("Start Date={0}", startDate.ToShortDateString());
                Logger.Info("Service Type={0}", serviceType);
            }

            return mr;
        }


        public Response CheckFriScore(string friScore)
        {
            Response r;

            try
            {
                if (DiagCodeAdapter.ValidateFriScore(friScore))
                {
                    r = new Response{IsValid = true, ErrorMessage = string.Empty};
                }
                else
                {
                    r = new Response{IsValid = false, ErrorMessage = "Invalid FRI Score"};
                }

            }
            catch (Exception ex)
            {
                r = new Response{IsValid = false, ErrorMessage = ex.Message};
            }

            return r;
        }


        public Response CheckPsfsScore(string psfsScore)
        {
            Response r;
            const string pat = @"^\d{1,2}(\.\d)?$";

            try
            {
                //if (DiagCodeAdapter.ValidatePsfsScore(psfsScore))
                //{
                //    r = new Response { IsValid = true, ErrorMessage = string.Empty };
                //}
                //else
                //{
                //    r = new Response { IsValid = false, ErrorMessage = "Invalid PSFS Score" };
                //}

                var reg = new Regex(pat, RegexOptions.IgnoreCase);
                if (reg.IsMatch(psfsScore))
                {
                    try
                    {
                        var score = decimal.Parse(psfsScore);
                        if (score >= 0m && score <= 10m)
                        {
                            r = new Response { IsValid = true, ErrorMessage = string.Empty };
                        }
                        else
                        {
                            r = new Response { IsValid = false, ErrorMessage = "Invalid PSFS Score" };
                        }
                    }
                    catch (Exception)
                    {
                        r = new Response { IsValid = false, ErrorMessage = "Invalid PSFS Score" };
                    }
                }
                else
                {
                    r = new Response { IsValid = false, ErrorMessage = "Invalid PSFS Score" };
                }
            }
            catch (Exception ex)
            {
                r = new Response { IsValid = false, ErrorMessage = ex.Message };
            }

            return r;
        }


        public Response CheckMedicareDiagCode(string code, int codeType)
        {
            Response r;

            try
            {
                if (DiagCodeAdapter.ValidateMedicareDiagCode(code, codeType))
                {
                    r = new Response { IsValid = true, ErrorMessage = string.Empty };
                }
                else
                {
                    r = new Response { IsValid = false, ErrorMessage = "Invalid Medicare Advantage Diagnosis Code" };
                }

            }
            catch (Exception ex)
            {
                r = new Response { IsValid = false, ErrorMessage = ex.Message };
            }

            return r;
        }


        public AuthResultResponse GetAuthResult(string authNumber)
        {
            return AuthAdapter.GetAuthResult(authNumber);
        }


        public ProviderResponse GetProviderInfo(string ivrCode)
        {
            return ProviderAdapter.ViewProviderInfo(ivrCode);
        }


        public ProviderResponse GetProviderFax(string providerId)
        {
            return ProviderAdapter.GetProviderFax(providerId);
        }


        public Response ValidateBcbsmaNumber(string bcbsmaNumber, string providerId, string subscriberId, string memberSeq)
        {
            Response r;

            try
            {
                if (AuthAdapter.IsValidBcbsmaNumber(bcbsmaNumber, providerId, subscriberId, memberSeq))
                {
                    r = new Response { IsValid = true, ErrorMessage = string.Empty };
                }
                else
                {
                    r = new Response { IsValid = false, ErrorMessage = "Invalid BCBSMA prior auth number" };
                }

            }
            catch (Exception ex)
            {
                r = new Response { IsValid = false, ErrorMessage = ex.Message };
            }

            return r;
        }


        public List<string> GetDiagnosticCodes(string keyWord)
        {
            var result = new List<string>();

            try
            {
                result = DiagCodeAdapter.GetDiagnosticCodes(keyWord);
            }
            catch (Exception ex)
            {
                result.Add(ex.Message);
            }

            return result;
        }


        public DataSet GetProviderAddressList(string providerId)
        {
            return ProviderAdapter.GetAddressList(providerId);
        }


        public bool InsertInvalidDiagCode(string code, string providerId)
        {
            return DiagCodeAdapter.InsertInvalidDiagCode(code, providerId);
        }


        public bool HasDuplicateClientAuthNumber(string clientAuthNumber, string subscriberId, string providerId)
        {
            var result = false;

            try
            {
                if (AuthAdapter.CheckDuplicateClientAuthNumber(clientAuthNumber, subscriberId, providerId))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = true;
            }

            //var logTime = DateTime.Now;
            //Logger.Info("*************************************   Check Duplicate Client Auth Number occurred at {0} {1}   *******************************************", logTime.ToShortDateString(), logTime.ToShortTimeString());
            //Logger.Info("The original submitted data for the above Check Duplicate Client Auth Number are listed below:");
            //Logger.Info("Client Auth Number={0}", clientAuthNumber);
            //Logger.Info("Subscriber ID={0}", subscriberId);
            //Logger.Info("Provider ID={0}", providerId);

            return result;
        }


        public HighmarkMemberResponse GetHighmarkMemberData(string subscriberId, DateTime inquiryDate)
        {
            return MemberAdapter.GetHighmarkMemberData(subscriberId, string.Empty, inquiryDate);
        }


        public HighmarkMemberResponse GetHighmarkMemberDataByAuth(string authNumber, DateTime inquiryDate)
        {
            return MemberAdapter.GetHighmarkMemberDataByAuth(authNumber, inquiryDate);
        }


        public DataSet GetProviderAddressListFromSso(string providerId, DateTime requestedStartDate)
        {
            return ProviderAdapter.GetAddressListFromSso(providerId, requestedStartDate);
        }


        public DataSet GetAllMatchedPtotAuthType(string ivrCode, string subscriberId, string memberSeq, DateTime requestedStartDate)
        {
            return ProviderAdapter.GetAllMatchedPtotAuthType(ivrCode, subscriberId, memberSeq, requestedStartDate);
        }


        public DataSet GetAllMatchedPtotAuthTypeFromSso(string ivrCode, string clientPrefix, string subscriberId, string blindKey, DateTime requestedStartDate)
        {
            return ProviderAdapter.GetAllMatchedPtotAuthTypeFromSso(ivrCode, clientPrefix, subscriberId, blindKey, requestedStartDate);
        }


        public DataSet GetAllServiceType(string ivrCode, string prefixSubscriberId, string memberSeq, DateTime requestedStartDate)
        {
            return ProviderAdapter.GetAllServiceType(ivrCode, prefixSubscriberId, memberSeq, requestedStartDate);
        }


        public MemberResponse GetClaimsAddress(string ivrCode, string prefixSubscriberId, string memberSeq)
        {
            return MemberAdapter.GetClaimsAddress(ivrCode, prefixSubscriberId, memberSeq);
        }


        public DataSet GetBcbsmaProviderList()
        {
            return ProviderAdapter.GetBcbsmaProviderList();
        }


        public BcbsmaSsoResponse ValidateBcbsmaSso(string bcbsmaProviderId)
        {
            var dict = new Dictionary<string, string>();
            var vr = ProviderController.ValidateBcbsmaSso(bcbsmaProviderId, ref dict);
            BcbsmaSsoResponse sso;

            if (vr.Success)
            {
                sso = new BcbsmaSsoResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    ProviderId = dict["Provider_ID"],
                    BcbsmaGroupIndicator = int.Parse(dict["Group_Indicator"]),
                    BcbsmaProviderType = dict["Provider_Type"],
                    IvrCode = dict["ivr_code"]
                };
            }
            else
            {
                sso = new BcbsmaSsoResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Validate BCBSMA SSO for BCBSMA Provider Id={0} has the following error: {1}", bcbsmaProviderId, vr.Reason);
                Logger.Info("The original submitted data for the above Validate BCBSMA SSO are listed below:");
                Logger.Info("BCBSMA Provider Id={0}", bcbsmaProviderId);
            }

            return sso;
        }


        public DataSet GetBcbsmaProviderGroup(string providerId)
        {
            return ProviderAdapter.GetBcbsmaProviderGroup(providerId);
        }


        public ProviderResponse GetBcbsmaProviderInfo(string providerId)
        {
            return ProviderAdapter.ViewBcbsmaProviderInfo(providerId);
        }


        public MemberResponse CheckBcbsmaMember(string providerId, string subscriberId, string memberSeq, DateTime dateOfBirth, DateTime startDate)
        {
            var dict = new Dictionary<string, string>();
            var vr = MemberController.ValidateBcbsmaMember(providerId, subscriberId, memberSeq, 
                                            dateOfBirth, startDate, ref dict);
            MemberResponse mr;

            if (vr.Success)
            {
                mr = new MemberResponse
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    MemberName = dict["member_name"],
                    MaxVisits = int.Parse(dict["plan_max_visits"]),
                    ActualVisits = int.Parse(dict["actual_visits"]),
                    PlanId = dict["plan_id"],
                    ActualVisitStartDate = DateTime.Parse(dict["actual_start_date"]),
                    ActualVisitEndDate = DateTime.Parse(dict["actual_end_date"]),
                    VisitYearType = dict["visit_year_type"],
                    ClaimsAddressError = dict["claimsAddressError"],
                    ClaimsAddressId = int.Parse(dict["claimsAddressId"]),
                    ClaimsAddress = dict["claimsAddress"],
                    DateOfBirth = DateTime.Parse(dict["dob"]),
                    PrefixSubscriberId = dict["prefix_subscriber_id"],
                    IvrCode = dict["ivr_code"]
                };
            }
            else
            {
                mr = new MemberResponse
                {
                    IsValid = false,
                    ErrorMessage = vr.Reason
                };

                var errorTime = DateTime.Now;
                Logger.Error("*************************************   Error occurred at {0} {1}   *******************************************", errorTime.ToShortDateString(), errorTime.ToShortTimeString());
                Logger.Error("Check BCBSMA Member for ProviderId={0}, SubscriberId={1}, MemberSeq={2} has the following error: {3}", providerId, subscriberId, memberSeq, vr.Reason);
                Logger.Info("The original submitted data for the above BCBSMA Check Member are listed below:");
                Logger.Info("Provider Id={0}", providerId);
                Logger.Info("Subscriber Id={0}", subscriberId);
                Logger.Info("Member Seq={0}", memberSeq);
                Logger.Info("Date Of Birth={0}", dateOfBirth.ToShortDateString());
                Logger.Info("Start Date={0}", startDate.ToShortDateString());
            }

            return mr;
        }


        public string GetBcbsmaProviderReminder(string providerId, string subscriberId, string memberSeq, DateTime startDate)
        {
            return ProviderAdapter.GetBcbsmaProviderReminder(providerId, subscriberId, memberSeq, startDate);
        }
    }
}
