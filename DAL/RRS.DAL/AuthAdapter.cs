using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RRS.BEL;

namespace RRS.DAL
{
    public class AuthAdapter
    {
        public static Dictionary<string, string> GetCaseNumber(AuthRequest request, Member member, Provider provider)
        {
            var dict = new Dictionary<string, string>();
            var priorAuthNumber = string.Empty;
            var thirdDiagCode = string.Empty;

            if (member.AuthTypeId.Equals("DC"))
            {
                var chiroRequest = (ChiroAuthRequest)request;
                priorAuthNumber = DbHelper.NullString(chiroRequest.PriorAuthNumber);
                thirdDiagCode = DbHelper.NullString(chiroRequest.ThirdDiagnosisCode);
            }            
            
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_DetermineCaseNumber_Web", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_request_type", SqlDbType.VarChar)).Value = DbHelper.NullString(request.RequestType);
                    cmd.Parameters.Add(new SqlParameter("@pPrior_Auth_number", SqlDbType.VarChar)).Value = priorAuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_id", SqlDbType.VarChar)).Value = provider.Id;
                    cmd.Parameters.Add(new SqlParameter("@pProviderLocation_Seq", SqlDbType.VarChar)).Value = provider.LocationSeq;
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode;
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_2", SqlDbType.VarChar)).Value = DbHelper.NullString(request.SecondaryDiagnosisCode);
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_3", SqlDbType.VarChar)).Value = thirdDiagCode;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pWeeks_Requested", SqlDbType.VarChar)).Value = request.NumberOfWeeks;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    //output
                    var caseNum = cmd.Parameters.Add("@pCase_number", SqlDbType.VarChar, 20);
                    caseNum.Direction = ParameterDirection.Output;
                    var newNotes = cmd.Parameters.Add("@pNew_Notes", SqlDbType.VarChar, 400);
                    newNotes.Direction = ParameterDirection.Output;
                    var newOrExtended = cmd.Parameters.Add("@pNew_or_Extended", SqlDbType.Char, 1);
                    newOrExtended.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["case_num"] = DbHelper.NullString(caseNum.Value);
                    dict["new_notes"] = DbHelper.NullString(newNotes.Value);
                    dict["new_or_extended"] = DbHelper.NullString(newOrExtended.Value);
                }
            }

            return dict;
        }

        public static bool CheckDuplicateAuth(Member member, Provider provider, DateTime startDate)
        {
            var result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as dup_count from Auth " +
                                   "where subscriber_id=@pSubscriber_Id " +
                                   "and member_seq=@pMember_Seq " +
                                   "and provider_id=@pProvider_Id " +
                                   "and ProviderLocation_seq=@pProviderLocation_Seq " +
                                   "and AuthType_ID=@pAuthType_ID " +
                                   "and datediff(Day, Requested_From, @pRequested_From) = 0 " +
                                   "and datediff(minute, entry_date, getdate())<5";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_Id", SqlDbType.VarChar)).Value = provider.Id;
                    cmd.Parameters.Add(new SqlParameter("@pProviderLocation_Seq", SqlDbType.VarChar)).Value = provider.LocationSeq;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@pRequested_From", SqlDbType.DateTime)).Value = startDate;

                    conn.Open();
                    var dupCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (dupCount > 0)
                        result = true;
                }
            }

            return result;
        }

        public static int GetNextAuthNumber(int id)
        {
            int result;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "declare @iLast_Authorization int, @imore bit, @iauth varchar(20) " + Environment.NewLine +
                          "select @iLast_Authorization = Last_Authorization " +
                          "from CompanyData " +
                          "where Key_Value=@iKey_Value " + Environment.NewLine +
                          "set @iLast_Authorization=@iLast_Authorization+1 " + Environment.NewLine +
                          "set @imore=1 " + Environment.NewLine +
                          "while @imore=1 " + Environment.NewLine +
                          "begin " + Environment.NewLine +
                          "select @iauth=Auth_Number from Auth " +
                          "where Auth_Number=convert(varchar(20), @iLast_Authorization) " + Environment.NewLine +
                          "if @@rowcount < 1 set @imore=0 " +
                          "else set @iLast_Authorization=@iLast_Authorization+1 " + Environment.NewLine +
                          "end " + Environment.NewLine +
                          "update CompanyData set Last_Authorization=convert(varchar(20), @iLast_Authorization) " +
                          "where Key_Value=@iKey_Value " + Environment.NewLine +
                          "select @iLast_Authorization as NextAuthNumber";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@iKey_Value", SqlDbType.Int)).Value = id;

                    conn.Open();
                    result = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                }
            } 

            return result;
        }

        public static void InsertCareRegistration(AuthRequest request, Member member, Provider provider, AuthResponse response, PendCode pendCode)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql =
                    "insert into auth (Auth_Number, AuthType_ID, Subscriber_ID, Member_Seq, Group_ID, Division_ID, " + Environment.NewLine +
                    "Group_Coverage_Start, Network_ID, Provider_ID, ProviderLocation_Seq, Visits_Requested, Diagnostic_Code1, " + Environment.NewLine +
                    "New_Or_Extended, Visits_Approved, Approved_From, Approved_Thru, Auth_Code, Approved_By, " + Environment.NewLine +
                    "Entry_Date, Entry_User, Update_Date, Update_User, Status, DRG_Category, DRG_Code, " + Environment.NewLine +
                    "Requested_From, Requested_Thru, Approved_Date, Plan_ID, IVR_Flag, ClientAuthNumber, ApplicationId, " + Environment.NewLine +
                    "Ordering_Provider_Name, Ordering_Provider_Address, Ordering_Provider_City, Ordering_Provider_State, Ordering_Provider_Zip_Code, " + Environment.NewLine +
                    "Ordering_Provider_Phone, Ordering_Provider_Fax, Ordering_Provider_Diagnosis, Referral_Date, Case_Number, SelectedServiceType, Preliminary_Visits_Requested) " + Environment.NewLine + 
                    "VALUES (@pAuth_Number, @pAuthType_ID, @pSubscriber_ID, @pMember_Seq, @pGroup_ID, @pDivision_ID, " + Environment.NewLine +
                    "@pGroup_Coverage_Start, @pNetwork_ID, @pProvider_ID, @pProviderLocation_Seq, @pVisits_Requested, @pDiagnostic_Code1, " + Environment.NewLine +
                    "1, @pVisits_Approved, @pApproved_From, @pApproved_Thru, @pAuth_Code, @pApproved_By, " + Environment.NewLine +
                    "getdate(), 'IVR', getdate(), 'IVR', @pStatus, @pDRG_Category, @pDRG_Code, " + Environment.NewLine +
                    "@pRequested_From, @pRequested_Thru, getdate(), @pPlan_ID, '1', @pClientAuthNumber, @pApplicationId, " + Environment.NewLine +
                    "@pOrdering_Provider_Name, @pOrdering_Provider_Address, @pOrdering_Provider_City, @pOrdering_Provider_State, @pOrdering_Provider_Zip_Code, " + Environment.NewLine +
                    "@pOrdering_Provider_Phone, @pOrdering_Provider_Fax, @pOrdering_Provider_Diagnosis, @pReferral_Date, @pCase_Number, @pSelectedServiceType, @pPreliminary_Visits_Requested)";

                var endOfStartDateYear = new DateTime(request.StartDate.Year, 12, 31);

                // Highmark Medicare Advantage member end date special logic
                // Also apply to Dean CRA
                if ((member.IsMedicareAdvantageMember && request.SubscriberId.StartsWith("HIGH")) || request.SubscriberId.StartsWith("DEAN"))
                {
                    var endDate = request.StartDate.AddDays(60);
                    if (endDate < endOfStartDateYear)
                    {
                        endOfStartDateYear = endDate;
                    }
                }

                if (pendCode.AuthCode == "CRA")
                {
                    response.DateApprovedThru = endOfStartDateYear;                    
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Number", SqlDbType.VarChar)).Value = response.AuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_ID", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_ID", SqlDbType.VarChar)).Value = member.GroupId;
                    cmd.Parameters.Add(new SqlParameter("@pDivision_ID", SqlDbType.VarChar)).Value = member.DivisionId;

                    cmd.Parameters.Add(new SqlParameter("@pGroup_Coverage_Start", SqlDbType.DateTime)).Value = member.GroupCoverageStart;
                    cmd.Parameters.Add(new SqlParameter("@pNetwork_ID", SqlDbType.VarChar)).Value = provider.NetworkId;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_ID", SqlDbType.VarChar)).Value = provider.Id;
                    cmd.Parameters.Add(new SqlParameter("@pProviderLocation_Seq", SqlDbType.VarChar)).Value = provider.LocationSeq;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Requested", SqlDbType.Int)).Value = member.CareRegistrationVisits;
                    cmd.Parameters.Add(new SqlParameter("@pDiagnostic_Code1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode.ToUpper();

                    cmd.Parameters.Add(new SqlParameter("@pVisits_Approved", SqlDbType.Int)).Value = response.VisitsApproved;
                    cmd.Parameters.Add(new SqlParameter("@pApproved_From", SqlDbType.DateTime)).Value = 
                        response.DateApprovedFrom.Equals(new DateTime(1900, 1, 1)) ? (object)DBNull.Value : response.DateApprovedFrom; 
                    cmd.Parameters.Add(new SqlParameter("@pApproved_Thru", SqlDbType.DateTime)).Value =
                        response.DateApprovedThru.Equals(new DateTime(1900, 1, 1)) ? (object)DBNull.Value : response.DateApprovedThru;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Code", SqlDbType.VarChar)).Value = response.AuthCode;
                    cmd.Parameters.Add(new SqlParameter("@pApproved_By", SqlDbType.VarChar)).Value = member.ApporvedBy;

                    cmd.Parameters.Add(new SqlParameter("@pStatus", SqlDbType.VarChar)).Value = response.AuthStatus;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Category", SqlDbType.VarChar)).Value = provider.Suffix;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Code", SqlDbType.VarChar)).Value = member.DrgCode;

                    cmd.Parameters.Add(new SqlParameter("@pRequested_From", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pRequested_Thru", SqlDbType.DateTime)).Value = endOfStartDateYear;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    cmd.Parameters.Add(new SqlParameter("@pClientAuthNumber", SqlDbType.VarChar)).Value = DbHelper.NullString(request.ClientAuthNumber);
                    cmd.Parameters.Add(new SqlParameter("@pApplicationId", SqlDbType.Int)).Value = request.ApplicationId;

                    var ptotRequest = request as PtotAuthRequest;
                    if (ptotRequest != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Name", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderName);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Address", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderAddress);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_City", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderCity);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_State", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderState);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Zip_Code", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderZipCode);

                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Phone", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderPhone);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Fax", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderFax);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Diagnosis", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderDiagnosis);
                        cmd.Parameters.Add(new SqlParameter("@pReferral_Date", SqlDbType.DateTime)).Value = DbHelper.NullDateTime(ptotRequest.ReferralDate);
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Name", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Address", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_City", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_State", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Zip_Code", SqlDbType.VarChar)).Value = DBNull.Value;

                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Phone", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Fax", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Diagnosis", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pReferral_Date", SqlDbType.DateTime)).Value = DBNull.Value;                        
                    }

                    cmd.Parameters.Add(new SqlParameter("@pCase_Number", SqlDbType.VarChar)).Value = DbHelper.NullString(member.CaseNumber);
                    cmd.Parameters.Add(new SqlParameter("@pSelectedServiceType", SqlDbType.VarChar)).Value = DbHelper.NullString(member.SelectedServiceType);
                    cmd.Parameters.Add(new SqlParameter("@pPreliminary_Visits_Requested", SqlDbType.Int)).Value = member.CareRegistrationVisits;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertAuthForChiro(ChiroAuthRequest request, Member member, Provider provider, 
                                AuthResponse response)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                // Manipulate Request Data 
                var newOrRecurringInjury = 0;
                if (request.Condition.Equals("2"))
                    newOrRecurringInjury = 1;

                var autoAccident = "0";
                if (request.AutoAccident)
                    autoAccident = "1";

                var workRelated = "0";
                if (request.WorkRelated)
                    workRelated = "1";

                //var otherInsurance = "0";
                //if (request.InjuryType.Equals("3"))
                //{
                //    otherInsurance = "1";
                //}

                var sql = "insert into auth (Auth_Number, Subscriber_ID, Member_Seq, Provider_ID, ProviderLocation_Seq, Visits_Requested, Other_Insurance, New_Or_Extended, " + Environment.NewLine +
                    "requested_from, requested_thru, closed, Previous_Treatment, Approved_From, Approved_Thru, " + Environment.NewLine +
                    "Visits_Approved, auth_code, Approved_Date, Approved_by, Notes, Work_related, Auto_Accident, " + Environment.NewLine +
                    "Audit_flag, status, Injury_Date, DRG_Code, Region, entry_date, entry_user, update_date, update_user, " + Environment.NewLine +
                    "Visits_to_date, additional_visits, IVR_Flag, Plan_id, score, fri_score, Diagnostic_Code1, " + Environment.NewLine +
                    "Diagnostic_Code2, Diagnostic_3, DRG_Category, group_id, division_id, Group_Coverage_Start, Network_ID, " + Environment.NewLine +
                    "AuthType_ID, Denial_Letter_Printed, Denial_Letter_Date, Denial_Letter_User, Diabetes, Stroke, Cancer, Obesity, " + Environment.NewLine +
                    "Smoker, Chronic_Pain, Case_Number, BCBSMA_number, Exercise, Depression, ApplicationId, ClientAuthNumber, SelectedServiceType, Preliminary_Visits_Requested, " + Environment.NewLine +
                    "Other_Injury, Post_Surgery, No_Injury, Duration, Limit_Communication, Pain_History, PSFS_Score, Condition) " + Environment.NewLine + 
                    "VALUES (@pAuth_Num, @pSubscriber_Id, @pMember_Seq, @pProvider_Id, @pProviderLocation_Seq, @pVisits_Requested, @iOtherInsurance, @iAuth_request_type, " + Environment.NewLine +
                    "@pStart_Date_Requested, @iRequest_thru, @iClosed, @iNew_or_recurring_injury, @iApprovedFrom, @iApprovedThru, " + Environment.NewLine +
                    "@iVisitsApproved, @iAuth_code, getdate(), @iApproved_by, @iNotes, @iWorkRelated, @iAutoAccident, " + Environment.NewLine +
                    "@pAuditFlag, @istatus, @pInitial_Injury_date, @pDRG_Code, ' ', getdate(), 'IVR', getdate(), 'IVR', " + Environment.NewLine +
                    "@pPrior_treatment_visits, @iAdditionalVisits, '1', @pPlan_ID, @pScore, @pFRI_Score, @pICD9_code_1, " + Environment.NewLine +
                    "@pICD9_code_2, @pICD9_code_3, @pProv_Suffix, @pGroup_ID, @pDivision_ID, @pGroup_Coverage_start, @pNetwork_ID, " + Environment.NewLine +
                    "@pAuthType_ID, @iDenial_letter_printed, @iDenial_letter_date, @iDenial_letter_User, @pDiabetes, @pStroke, " + Environment.NewLine +
                    "@pCancer, @pObesity, @pSmoker, @pChronic_pain, @iCase_number, @pbcbsma_number, @pExercise, @pDepression, @pApplicationId, @pClientAuthNumber, @pSelectedServiceType, @pPreliminary_Visits_Requested, " + Environment.NewLine +
                    "@pOther_Injury, @pPost_Surgery, @pNo_Injury, @pDuration, @pLimit_Communication, @pPain_History, @pPSFS_Score, @pCondition)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Num", SqlDbType.VarChar)).Value = response.AuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_Id", SqlDbType.VarChar)).Value = provider.Id;
                    cmd.Parameters.Add(new SqlParameter("@pProviderLocation_Seq", SqlDbType.VarChar)).Value = provider.LocationSeq;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Requested", SqlDbType.Int)).Value = member.VisitsRequested;
                    cmd.Parameters.Add(new SqlParameter("@iOtherInsurance", SqlDbType.VarChar)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@iAuth_request_type", SqlDbType.Bit)).Value = member.NewOrExtended;

                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@iRequest_thru", SqlDbType.DateTime)).Value = request.StartDate.AddDays(request.NumberOfWeeks*7);
                    cmd.Parameters.Add(new SqlParameter("@iClosed", SqlDbType.Bit)).Value = 0;
                    cmd.Parameters.Add(new SqlParameter("@iNew_or_recurring_injury", SqlDbType.Bit)).Value = newOrRecurringInjury;
                    cmd.Parameters.Add(new SqlParameter("@iApprovedFrom", SqlDbType.DateTime)).Value =
                        response.DateApprovedFrom.Equals(new DateTime(1900, 1, 1)) ? (object) DBNull.Value : response.DateApprovedFrom;
                    cmd.Parameters.Add(new SqlParameter("@iApprovedThru", SqlDbType.DateTime)).Value =
                        response.DateApprovedThru.Equals(new DateTime(1900, 1, 1)) ? (object) DBNull.Value : response.DateApprovedThru;

                    cmd.Parameters.Add(new SqlParameter("@iVisitsApproved", SqlDbType.Int)).Value = response.VisitsApproved;
                    cmd.Parameters.Add(new SqlParameter("@iAuth_code", SqlDbType.VarChar)).Value = response.AuthCode;
                    cmd.Parameters.Add(new SqlParameter("@iApproved_by", SqlDbType.VarChar)).Value = member.ApporvedBy;
                    cmd.Parameters.Add(new SqlParameter("@iNotes", SqlDbType.VarChar)).Value = member.AuthNotes;
                    cmd.Parameters.Add(new SqlParameter("@iWorkRelated", SqlDbType.VarChar)).Value = workRelated;
                    cmd.Parameters.Add(new SqlParameter("@iAutoAccident", SqlDbType.VarChar)).Value = autoAccident;

                    cmd.Parameters.Add(new SqlParameter("@pAuditFlag", SqlDbType.VarChar)).Value = member.AuditFlag;
                    cmd.Parameters.Add(new SqlParameter("@istatus", SqlDbType.VarChar)).Value = response.AuthStatus;
                    cmd.Parameters.Add(new SqlParameter("@pInitial_Injury_date", SqlDbType.DateTime)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Code", SqlDbType.VarChar)).Value = member.DrgCode;

                    cmd.Parameters.Add(new SqlParameter("@pPrior_treatment_visits", SqlDbType.Int)).Value = request.VisitsToDate;
                    cmd.Parameters.Add(new SqlParameter("@iAdditionalVisits", SqlDbType.Int)).Value = member.AdditionalVisits;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    cmd.Parameters.Add(new SqlParameter("@pScore", SqlDbType.Int)).Value = member.CurrentScore;
                    cmd.Parameters.Add(new SqlParameter("@pFRI_Score", SqlDbType.Int)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode.ToUpper();

                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_2", SqlDbType.VarChar)).Value = request.SecondaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_3", SqlDbType.VarChar)).Value = request.ThirdDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pProv_Suffix", SqlDbType.VarChar)).Value = provider.Suffix;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_ID", SqlDbType.VarChar)).Value = member.GroupId;
                    cmd.Parameters.Add(new SqlParameter("@pDivision_ID", SqlDbType.VarChar)).Value = member.DivisionId;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_Coverage_start", SqlDbType.DateTime)).Value = member.GroupCoverageStart;
                    cmd.Parameters.Add(new SqlParameter("@pNetwork_ID", SqlDbType.VarChar)).Value = provider.NetworkId;

                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@iDenial_letter_printed", SqlDbType.VarChar)).Value = member.DenialLetterPrinted;
                    cmd.Parameters.Add(new SqlParameter("@iDenial_letter_date", SqlDbType.DateTime)).Value = member.DenialLetterDate;
                    cmd.Parameters.Add(new SqlParameter("@iDenial_letter_User", SqlDbType.VarChar)).Value = member.DenialLetterUser;
                    cmd.Parameters.Add(new SqlParameter("@pDiabetes", SqlDbType.Bit)).Value = request.HasDiabetes ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pStroke", SqlDbType.Bit)).Value = request.HasStroke ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pCancer", SqlDbType.Bit)).Value = request.HasCancer ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pObesity", SqlDbType.Bit)).Value = request.IsOverweight ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pSmoker", SqlDbType.Bit)).Value = request.IsSmoker ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pChronic_pain", SqlDbType.Bit)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@iCase_number", SqlDbType.VarChar)).Value = member.CaseNumber;
                    cmd.Parameters.Add(new SqlParameter("@pbcbsma_number", SqlDbType.VarChar)).Value = string.Empty;
                    cmd.Parameters.Add(new SqlParameter("@pExercise", SqlDbType.Bit)).Value = request.HasExercise ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDepression", SqlDbType.Bit)).Value = request.HasDepression ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pApplicationId", SqlDbType.Int)).Value = request.ApplicationId;
                    cmd.Parameters.Add(new SqlParameter("@pClientAuthNumber", SqlDbType.VarChar)).Value = DbHelper.NullString(request.ClientAuthNumber);
                    cmd.Parameters.Add(new SqlParameter("@pSelectedServiceType", SqlDbType.VarChar)).Value = DbHelper.NullString(member.SelectedServiceType);

                    cmd.Parameters.Add(new SqlParameter("@pPreliminary_Visits_Requested", SqlDbType.Int)).Value = request.NumberOfVisits;

                    cmd.Parameters.Add(new SqlParameter("@pOther_Injury", SqlDbType.Bit)).Value = request.OtherInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPost_Surgery", SqlDbType.Bit)).Value = request.PostSurgery ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pNo_Injury", SqlDbType.Bit)).Value = request.NoInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDuration", SqlDbType.VarChar)).Value = request.Duration;
                    cmd.Parameters.Add(new SqlParameter("@pLimit_Communication", SqlDbType.Bit)).Value = request.LimitCommunication ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPain_History", SqlDbType.Bit)).Value = request.HasPainHistory ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPSFS_Score", SqlDbType.Decimal)).Value = (request.PsfsScore == -1m) ? (object)DBNull.Value : request.PsfsScore;
                    cmd.Parameters.Add(new SqlParameter("@pCondition", SqlDbType.Int)).Value = DbHelper.NullInt(request.Condition);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertAuthForPtot(PtotAuthRequest request, Member member, Provider provider,
                                AuthResponse response)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                // Manipulate Request Data 
                var newOrRecurringInjury = 0;
                if (request.Condition.Equals("2"))
                    newOrRecurringInjury = 1;

                var autoAccident = "0";
                if (request.AutoAccident)
                    autoAccident = "1";

                var workRelated = "0";
                if (request.WorkRelated)
                    workRelated = "1";

                //var otherInsurance = 0;
                //if (request.InjuryType.Equals("3"))
                //{
                //    otherInsurance = 1;
                //}

                var sql = "insert into auth (Auth_Number, Subscriber_ID, Member_Seq, Provider_ID, ProviderLocation_Seq, Visits_Requested, New_Or_Extended, " + Environment.NewLine +
                    "requested_from, requested_thru, closed, Previous_Treatment, Approved_From, Approved_Thru, " + Environment.NewLine +
                    "Visits_Approved, auth_code, Approved_Date, Approved_by, Notes, Work_related, Auto_Accident, " + Environment.NewLine +
                    "Audit_flag, status, DRG_Code, Region, entry_date, entry_user, update_date, update_user, " + Environment.NewLine +
                    "Visits_to_date, additional_visits, IVR_Flag, Plan_id, score, Diagnostic_Code1, " + Environment.NewLine +
                    "Diagnostic_Code2, Diagnostic_3, DRG_Category, group_id, division_id, Group_Coverage_Start, Network_ID, " + Environment.NewLine +
                    "AuthType_ID, Denial_Letter_Printed, Denial_Letter_Date, Denial_Letter_User, Diabetes, " + Environment.NewLine +
                    "Cancer, Obesity, Smoker, Case_Number, BCBSMA_number, " + Environment.NewLine +
                    "Ordering_Provider_Name, Ordering_Provider_Address, Ordering_Provider_City, Ordering_Provider_State, Ordering_Provider_Zip_Code, " + Environment.NewLine +
                    "Ordering_Provider_Phone, Ordering_Provider_Fax, Ordering_Provider_Diagnosis, Referral_Date, " + Environment.NewLine + 
                    "Other_Injury, Post_Surgery, No_Injury, Duration, Prior_Treatment, " + Environment.NewLine +
                    "Upper_Extremity, Lower_Extremity, LS_Spine, CT_Spine, Hand_Wrist, Other_Body_Region, " + Environment.NewLine +
                    "PSFS_Score, Pain_History, Drinker, Opioids, Exercise, Confident, " + Environment.NewLine +
                    "Depression, Limit_Communication, CNS, Cardiovascular_Condition, Lung_Disease, ApplicationId, ClientAuthNumber, SelectedServiceType, Preliminary_Visits_Requested, Condition) " + Environment.NewLine + 
                    "VALUES (@pAuth_Num, @pSubscriber_Id, @pMember_Seq, @pProvider_Id, @pProviderLocation_Seq, @pVisits_Requested, @iAuth_request_type, " + Environment.NewLine +
                    "@pStart_Date_Requested, @iRequest_thru, @iClosed, @iNew_or_recurring_injury, @iApprovedFrom, @iApprovedThru, " + Environment.NewLine +
                    "@iVisitsApproved, @iAuth_code, getdate(), @iApproved_by, @iNotes, @iWorkRelated, @iAutoAccident, " + Environment.NewLine +
                    "@pAuditFlag, @istatus, @pDRG_Code, ' ', getdate(), 'IVR', getdate(), 'IVR', " + Environment.NewLine +
                    "@pPrior_treatment_visits, @iAdditionalVisits, '1', @pPlan_ID, @pScore, @pICD9_code_1, " + Environment.NewLine +
                    "@pICD9_code_2, @pICD9_code_3, @pProv_Suffix, @pGroup_ID, @pDivision_ID, @pGroup_Coverage_start, @pNetwork_ID, " + Environment.NewLine +
                    "@pAuthType_ID, @iDenial_letter_printed, @iDenial_letter_date, @iDenial_letter_User, @pDiabetes, " + Environment.NewLine +
                    "@pCancer, @pObesity, @pSmoker, @iCase_number, @pbcbsma_number, " + Environment.NewLine + 
                    "@pOrdering_Provider_Name, @pOrdering_Provider_Address, @pOrdering_Provider_City, @pOrdering_Provider_State, @pOrdering_Provider_Zip_Code, " + Environment.NewLine +
                    "@pOrdering_Provider_Phone, @pOrdering_Provider_Fax, @pOrdering_Provider_Diagnosis, @pReferral_Date, " + Environment.NewLine +
                    "@pOther_Injury, @pPost_Surgery, @pNo_Injury, @pDuration, @pPrior_Treatment, " + Environment.NewLine +
                    "@pUpper_Extremity, @pLower_Extremity, @pLS_Spine, @pCT_Spine, @pHand_Wrist, @pOther_Body_Region, " + Environment.NewLine +
                    "@pPSFS_Score, @pPain_History, @pDrinker, @pOpioids, @pExercise, @pConfident, " + Environment.NewLine +
                    "@pDepression, @pLimit_Communication, @pCNS, @pCardiovascular_Condition, @pLung_Disease, @pApplicationId, @pClientAuthNumber, @pSelectedServiceType, @pPreliminary_Visits_Requested, @pCondition)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Num", SqlDbType.VarChar)).Value = response.AuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_Id", SqlDbType.VarChar)).Value = provider.Id;
                    cmd.Parameters.Add(new SqlParameter("@pProviderLocation_Seq", SqlDbType.VarChar)).Value = provider.LocationSeq;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Requested", SqlDbType.Int)).Value = member.VisitsRequested;
                    //cmd.Parameters.Add(new SqlParameter("@iOtherInsurance", SqlDbType.VarChar)).Value = otherInsurance;
                    cmd.Parameters.Add(new SqlParameter("@iAuth_request_type", SqlDbType.Bit)).Value = member.NewOrExtended;

                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@iRequest_thru", SqlDbType.DateTime)).Value = request.StartDate.AddDays(request.NumberOfWeeks * 7);
                    cmd.Parameters.Add(new SqlParameter("@iClosed", SqlDbType.Bit)).Value = 0;
                    cmd.Parameters.Add(new SqlParameter("@iNew_or_recurring_injury", SqlDbType.Bit)).Value = newOrRecurringInjury;
                    cmd.Parameters.Add(new SqlParameter("@iApprovedFrom", SqlDbType.DateTime)).Value =
                        response.DateApprovedFrom.Equals(new DateTime(1900, 1, 1)) ? (object)DBNull.Value : response.DateApprovedFrom;
                    cmd.Parameters.Add(new SqlParameter("@iApprovedThru", SqlDbType.DateTime)).Value =
                        response.DateApprovedThru.Equals(new DateTime(1900, 1, 1)) ? (object)DBNull.Value : response.DateApprovedThru;

                    cmd.Parameters.Add(new SqlParameter("@iVisitsApproved", SqlDbType.Int)).Value = response.VisitsApproved;
                    cmd.Parameters.Add(new SqlParameter("@iAuth_code", SqlDbType.VarChar)).Value = response.AuthCode;
                    cmd.Parameters.Add(new SqlParameter("@iApproved_by", SqlDbType.VarChar)).Value = member.ApporvedBy;
                    cmd.Parameters.Add(new SqlParameter("@iNotes", SqlDbType.VarChar)).Value = member.AuthNotes;
                    cmd.Parameters.Add(new SqlParameter("@iWorkRelated", SqlDbType.VarChar)).Value = workRelated;
                    cmd.Parameters.Add(new SqlParameter("@iAutoAccident", SqlDbType.VarChar)).Value = autoAccident;

                    cmd.Parameters.Add(new SqlParameter("@pAuditFlag", SqlDbType.VarChar)).Value = member.AuditFlag;
                    cmd.Parameters.Add(new SqlParameter("@istatus", SqlDbType.VarChar)).Value = response.AuthStatus;
                    //cmd.Parameters.Add(new SqlParameter("@pInitial_Injury_date", SqlDbType.DateTime)).Value = request.InitialInjuryDate;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Code", SqlDbType.VarChar)).Value = member.DrgCode;

                    cmd.Parameters.Add(new SqlParameter("@pPrior_treatment_visits", SqlDbType.Int)).Value = request.ActualVisits;
                    cmd.Parameters.Add(new SqlParameter("@iAdditionalVisits", SqlDbType.Int)).Value = member.AdditionalVisits;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    cmd.Parameters.Add(new SqlParameter("@pScore", SqlDbType.Int)).Value = member.CurrentScore;
                    //cmd.Parameters.Add(new SqlParameter("@pFRI_Score", SqlDbType.Int)).Value = request.FriScore;
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode.ToUpper();

                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_2", SqlDbType.VarChar)).Value = request.SecondaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_3", SqlDbType.VarChar)).Value = string.Empty;
                    cmd.Parameters.Add(new SqlParameter("@pProv_Suffix", SqlDbType.VarChar)).Value = provider.Suffix;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_ID", SqlDbType.VarChar)).Value = member.GroupId;
                    cmd.Parameters.Add(new SqlParameter("@pDivision_ID", SqlDbType.VarChar)).Value = member.DivisionId;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_Coverage_start", SqlDbType.DateTime)).Value = member.GroupCoverageStart;
                    cmd.Parameters.Add(new SqlParameter("@pNetwork_ID", SqlDbType.VarChar)).Value = provider.NetworkId;

                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@iDenial_letter_printed", SqlDbType.VarChar)).Value = member.DenialLetterPrinted;
                    cmd.Parameters.Add(new SqlParameter("@iDenial_letter_date", SqlDbType.DateTime)).Value = member.DenialLetterDate;
                    cmd.Parameters.Add(new SqlParameter("@iDenial_letter_User", SqlDbType.VarChar)).Value = member.DenialLetterUser;
                    cmd.Parameters.Add(new SqlParameter("@pDiabetes", SqlDbType.Bit)).Value = request.HasDiabetes ? 1 : 0;
                    //cmd.Parameters.Add(new SqlParameter("@pStroke", SqlDbType.Bit)).Value = request.HasStroke ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pCancer", SqlDbType.Bit)).Value = request.HasCancer ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pObesity", SqlDbType.Bit)).Value = request.IsOverweight ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pSmoker", SqlDbType.Bit)).Value = request.IsSmoker ? 1 : 0;
                    //cmd.Parameters.Add(new SqlParameter("@pChronic_pain", SqlDbType.Bit)).Value = request.HasChronicPain ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@iCase_number", SqlDbType.VarChar)).Value = member.CaseNumber;
                    cmd.Parameters.Add(new SqlParameter("@pbcbsma_number", SqlDbType.VarChar)).Value = string.Empty;

                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Name", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderName);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Address", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderAddress);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_City", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderCity);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_State", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderState);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Zip_Code", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderZipCode);

                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Phone", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderPhone);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Fax", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderFax);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Diagnosis", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderDiagnosis);
                    cmd.Parameters.Add(new SqlParameter("@pReferral_Date", SqlDbType.DateTime)).Value = DbHelper.NullDateTime(request.ReferralDate);

                    cmd.Parameters.Add(new SqlParameter("@pOther_Injury", SqlDbType.Bit)).Value = request.OtherInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPost_Surgery", SqlDbType.Bit)).Value = request.PostSurgery ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pNo_Injury", SqlDbType.Bit)).Value = request.NoInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDuration", SqlDbType.VarChar)).Value = request.Duration;
                    cmd.Parameters.Add(new SqlParameter("@pPrior_Treatment", SqlDbType.VarChar)).Value = request.PriorTreatment;

                    cmd.Parameters.Add(new SqlParameter("@pUpper_Extremity", SqlDbType.Bit)).Value = request.UpperExtremity ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLower_Extremity", SqlDbType.Bit)).Value = request.LowerExtremity ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLS_Spine", SqlDbType.Bit)).Value = request.LsSpine ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pCT_Spine", SqlDbType.Bit)).Value = request.CtSpine ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pHand_Wrist", SqlDbType.Bit)).Value = request.HandWrist ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pOther_Body_Region", SqlDbType.Bit)).Value = request.OtherRegion ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pPSFS_Score", SqlDbType.Decimal)).Value = (request.PsfsScore == -1m) ? (object)DBNull.Value : request.PsfsScore;
                    cmd.Parameters.Add(new SqlParameter("@pPain_History", SqlDbType.Bit)).Value = request.HasPainHistory ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDrinker", SqlDbType.Bit)).Value = request.IsDrinker ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pOpioids", SqlDbType.Bit)).Value = request.TakeOpioids ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pExercise", SqlDbType.Bit)).Value = request.HasExercise ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pConfident", SqlDbType.Bit)).Value = request.HasConfidence ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pDepression", SqlDbType.Bit)).Value = request.HasDepression ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLimit_Communication", SqlDbType.Bit)).Value = request.LimitCommunication ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pCNS", SqlDbType.Bit)).Value = request.HasCns ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pCardiovascular_Condition", SqlDbType.Bit)).Value = request.HasCardiovascular ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLung_Disease", SqlDbType.Bit)).Value = request.HasLungDisease ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pApplicationId", SqlDbType.Int)).Value = request.ApplicationId;
                    cmd.Parameters.Add(new SqlParameter("@pClientAuthNumber", SqlDbType.VarChar)).Value = DbHelper.NullString(request.ClientAuthNumber);
                    cmd.Parameters.Add(new SqlParameter("@pSelectedServiceType", SqlDbType.VarChar)).Value = DbHelper.NullString(member.SelectedServiceType);

                    cmd.Parameters.Add(new SqlParameter("@pPreliminary_Visits_Requested", SqlDbType.Int)).Value = request.NumberOfVisits;
                    cmd.Parameters.Add(new SqlParameter("@pCondition", SqlDbType.Int)).Value = DbHelper.NullInt(request.Condition);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static string GetBcbsmaNumber(int authNumber)
        {
            string result;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_bcbsma_determine_authorization_information", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pAuthNumber", SqlDbType.VarChar)).Value = authNumber;
                    //output
                    var bcbsmaNum = cmd.Parameters.Add("@pReturnAuth", SqlDbType.VarChar, 10);
                    bcbsmaNum.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    result = DbHelper.NullString(bcbsmaNum.Value);
                }
            }

            return result;
        }

        public static void ModifyPreviousEndDate(int authNumber)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_ModifyPreviousEndDate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pAuthNumber", SqlDbType.VarChar)).Value = authNumber;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }            
        }

        public static void InsertAuthDiagCodes(int authNumber, AuthRequest request, string authTypeId)
        {
            var secondDiagCode = DbHelper.NullString(request.SecondaryDiagnosisCode).ToUpper();
            var thirdDiagCode = string.Empty;

            if (authTypeId.Equals("DC"))
            {
                var chiroRequest = (ChiroAuthRequest)request;
                thirdDiagCode = DbHelper.NullString(chiroRequest.ThirdDiagnosisCode).ToUpper();
            } 

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "INSERT INTO AuthDiagCodes (Auth_Number, Diagnostic_Code, Primary_Diag) " +
                          "VALUES ('" + authNumber + "', '" + request.PrimaryDiagnosisCode.ToUpper() + "', '1') " + Environment.NewLine;

                if (secondDiagCode.Length > 0 && request.PrimaryDiagnosisCode.ToUpper() != secondDiagCode)
                {
                    sql += "INSERT INTO AuthDiagCodes (Auth_Number, Diagnostic_Code) " +
                           "VALUES ('" + authNumber + "', '" + secondDiagCode + "') " + Environment.NewLine;
                }

                if (thirdDiagCode.Length > 0 && request.PrimaryDiagnosisCode.ToUpper() != thirdDiagCode 
                        && secondDiagCode != thirdDiagCode)
                {
                    sql += "INSERT INTO AuthDiagCodes (Auth_Number, Diagnostic_Code) " +
                           "VALUES ('" + authNumber + "', '" + thirdDiagCode + "') " + Environment.NewLine;                    
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertCareRegistrationLog(AuthRequest request, Member member, AuthResponse response)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql =
                    "INSERT INTO Auth_Log (Auth_Number, Auth_Result, Auth_Date, IVR_Code, Member_ID, Member_Seq, " + Environment.NewLine +
                    "Auth_Level, Visits_Requested, Diag1, Visits_Approved, " + Environment.NewLine +
                    "Requested_From, ApplicationId, SelectedServiceType, " + Environment.NewLine +
                    "Ordering_Provider_Name, Ordering_Provider_Address, Ordering_Provider_City, Ordering_Provider_State, Ordering_Provider_Zip_Code, " + Environment.NewLine +
                    "Ordering_Provider_Phone, Ordering_Provider_Fax, Ordering_Provider_Diagnosis, Referral_Date, Preliminary_Visits_Requested) " + Environment.NewLine + 
                    "VALUES (@pAuth_Number, @pAuth_Result, getdate(), @pIVR_Code, @pMember_ID, @pMember_Seq, " + Environment.NewLine +
                    "1, @pVisits_Requested, @pDiag1, @pVisits_Approved, " + Environment.NewLine +
                    "@pRequested_From, @pApplicationId, @pSelectedServiceType, " + Environment.NewLine +
                    "@pOrdering_Provider_Name, @pOrdering_Provider_Address, @pOrdering_Provider_City, @pOrdering_Provider_State, @pOrdering_Provider_Zip_Code, " + Environment.NewLine +
                    "@pOrdering_Provider_Phone, @pOrdering_Provider_Fax, @pOrdering_Provider_Diagnosis, @pReferral_Date, @pPreliminary_Visits_Requested)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Number", SqlDbType.VarChar)).Value = response.AuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Result", SqlDbType.VarChar)).Value = response.AuthCode;
                    cmd.Parameters.Add(new SqlParameter("@pIVR_Code", SqlDbType.VarChar)).Value = request.IvrCode;
                    cmd.Parameters.Add(new SqlParameter("@pMember_ID", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;

                    cmd.Parameters.Add(new SqlParameter("@pVisits_Requested", SqlDbType.Int)).Value = member.CareRegistrationVisits;
                    cmd.Parameters.Add(new SqlParameter("@pDiag1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Approved", SqlDbType.Int)).Value = response.VisitsApproved;

                    cmd.Parameters.Add(new SqlParameter("@pRequested_From", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pApplicationId", SqlDbType.Int)).Value = request.ApplicationId;
                    cmd.Parameters.Add(new SqlParameter("@pSelectedServiceType", SqlDbType.VarChar)).Value = DbHelper.NullString(member.SelectedServiceType);

                    var ptotRequest = request as PtotAuthRequest;
                    if (ptotRequest != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Name", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderName);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Address", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderAddress);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_City", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderCity);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_State", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderState);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Zip_Code", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderZipCode);

                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Phone", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderPhone);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Fax", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderFax);
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Diagnosis", SqlDbType.VarChar)).Value = DbHelper.NullString(ptotRequest.OrderingProviderDiagnosis);
                        cmd.Parameters.Add(new SqlParameter("@pReferral_Date", SqlDbType.DateTime)).Value = DbHelper.NullDateTime(ptotRequest.ReferralDate);
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Name", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Address", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_City", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_State", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Zip_Code", SqlDbType.VarChar)).Value = DBNull.Value;

                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Phone", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Fax", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Diagnosis", SqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new SqlParameter("@pReferral_Date", SqlDbType.DateTime)).Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(new SqlParameter("@pPreliminary_Visits_Requested", SqlDbType.Int)).Value = member.CareRegistrationVisits;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertAuthLogForChiro(ChiroAuthRequest request, Member member, AuthResponse response)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                // Manipulate Request Data 
                var newOrRecurringInjury = 0;
                if (request.Condition.Equals("2"))
                    newOrRecurringInjury = 1;

                var treatedBy = 0;
                if (request.HasReferral)
                    treatedBy = 1;

                var coTreated = 0;
                if (request.IsCoTreat)
                    coTreated = 1;

                var sql =
                    "INSERT INTO Auth_Log(Auth_Number, Auth_Result, Auth_Date, IVR_Code, Member_ID, Member_Seq," + Environment.NewLine +
                    "Auth_Level, Onset_Date, Visits_Provided, Visits_Requested, " + Environment.NewLine +
                    "Weeks_Required, Diag1, Diag2, Diag3, NewOrRecurring, DailyLiving, " + Environment.NewLine +
                    "RadiatingPain, Motion, TreatedBy, Visits_Allowed, Visits_Approved, " + Environment.NewLine +
                    "Days_Allowed, Days_Approved, Score, Severity_Code, Previous_Auths, OnsetDatePoints, Diabetes, Stroke, " + Environment.NewLine +
                    "Cancer, Obesity, Smoker, Chronic_Pain, Initial_Injury_Visit_Date, Co_Treated, Previous_Auth_Num, " + Environment.NewLine +
                    "Patient_Insurance_Id, Care_Plan_Auth_Entry, Requested_From, Exercise, Depression, ApplicationId, SelectedServiceType, Preliminary_Visits_Requested, " + Environment.NewLine +
                    "Other_Injury, Post_Surgery, No_Injury, Duration, Limit_Communication, Pain_History, PSFS_Score, Condition) " + Environment.NewLine + 
                    "VALUES(@pAuth_Num, @iAuth_code, getdate(), @pIvr_code, @pSubscriber_Id, @pMember_Seq, " + Environment.NewLine +
                    "@pAuth_request_type, @pInitial_Injury_date, @pPrior_treatment_visits, @pVisits_Requested, " + Environment.NewLine +
                    "@pWeeks_Requested, @pICD9_code_1, @pICD9_code_2, @pICD9_code_3, @iNew_or_recurring_injury, @pDaily_Living_Rating, " + Environment.NewLine +
                    "@pPain_Rating, @pRange_of_Motion_Rating, @iTreatedBy, @pVisits_Allowed, @iVisitsApproved, " + Environment.NewLine +
                    "@pDays_Allowed,  @iDays_Allowed, @pScore, @pSevereCode, @pPreviousAuths, @pOnSetDatePoints, @pDiabetes, @pStroke, " + Environment.NewLine +
                    "@pCancer, @pObesity, @pSmoker, @pChronic_pain, @iInitial_Visit_date, @iCo_Treated, @pPrior_Auth_number, " + Environment.NewLine +
                    "@pPatient_Id, @pPlan_Auth_Entry, @pStart_Date_Requested, @pExercise, @pDepression, @pApplicationId, @pSelectedServiceType, @pPreliminary_Visits_Requested, " + Environment.NewLine +
                    "@pOther_Injury, @pPost_Surgery, @pNo_Injury, @pDuration, @pLimit_Communication, @pPain_History, @pPSFS_Score, @pCondition)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Num", SqlDbType.VarChar)).Value = response.AuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@iAuth_code", SqlDbType.VarChar)).Value = response.AuthCode;
                    cmd.Parameters.Add(new SqlParameter("@pIvr_code", SqlDbType.VarChar)).Value = request.IvrCode;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;

                    // old codes use request.RequestType ???
                    cmd.Parameters.Add(new SqlParameter("@pAuth_request_type", SqlDbType.Int)).Value = member.NewOrExtended;

                    cmd.Parameters.Add(new SqlParameter("@pInitial_Injury_date", SqlDbType.DateTime)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@pPrior_treatment_visits", SqlDbType.Int)).Value = request.VisitsToDate;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Requested", SqlDbType.Int)).Value = member.VisitsRequested;

                    cmd.Parameters.Add(new SqlParameter("@pWeeks_Requested", SqlDbType.Int)).Value = request.NumberOfWeeks;
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_2", SqlDbType.VarChar)).Value = request.SecondaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_3", SqlDbType.VarChar)).Value = request.ThirdDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@iNew_or_recurring_injury", SqlDbType.Int)).Value = newOrRecurringInjury;
                    cmd.Parameters.Add(new SqlParameter("@pDaily_Living_Rating", SqlDbType.Int)).Value = DBNull.Value;

                    cmd.Parameters.Add(new SqlParameter("@pPain_Rating", SqlDbType.Int)).Value = request.PainRating;
                    cmd.Parameters.Add(new SqlParameter("@pRange_of_Motion_Rating", SqlDbType.Int)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@iTreatedBy", SqlDbType.Int)).Value = treatedBy;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Allowed", SqlDbType.Int)).Value = member.AllowedVisits;
                    cmd.Parameters.Add(new SqlParameter("@iVisitsApproved", SqlDbType.Int)).Value = response.VisitsApproved;

                    cmd.Parameters.Add(new SqlParameter("@pDays_Allowed", SqlDbType.Int)).Value = member.AllowedDays;
                    // old codes use 0 which is definately wrong ???
                    cmd.Parameters.Add(new SqlParameter("@iDays_Allowed", SqlDbType.Int)).Value = member.ApprovedDays;
                    cmd.Parameters.Add(new SqlParameter("@pScore", SqlDbType.Int)).Value = member.CurrentScore;
                    cmd.Parameters.Add(new SqlParameter("@pSevereCode", SqlDbType.VarChar)).Value = member.SevereCode;
                    cmd.Parameters.Add(new SqlParameter("@pPreviousAuths", SqlDbType.Int)).Value = member.NumberOfPreviousAuths;
                    cmd.Parameters.Add(new SqlParameter("@pOnSetDatePoints", SqlDbType.Int)).Value = member.OnSetDatePoints;
                    cmd.Parameters.Add(new SqlParameter("@pDiabetes", SqlDbType.Bit)).Value = request.HasDiabetes ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pStroke", SqlDbType.Bit)).Value = request.HasStroke ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pCancer", SqlDbType.Bit)).Value = request.HasCancer ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pObesity", SqlDbType.Bit)).Value = request.IsOverweight ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pSmoker", SqlDbType.Bit)).Value = request.IsSmoker ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pChronic_pain", SqlDbType.Bit)).Value = DBNull.Value;
                    cmd.Parameters.Add(new SqlParameter("@iInitial_Visit_date", SqlDbType.DateTime)).Value = request.InitialVisitDate;
                    cmd.Parameters.Add(new SqlParameter("@iCo_Treated", SqlDbType.Bit)).Value = coTreated;
                    cmd.Parameters.Add(new SqlParameter("@pPrior_Auth_number", SqlDbType.VarChar)).Value = request.PriorAuthNumber;

                    cmd.Parameters.Add(new SqlParameter("@pPatient_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_Auth_Entry", SqlDbType.VarChar)).Value = request.RequestType;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pExercise", SqlDbType.Bit)).Value = request.HasExercise ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDepression", SqlDbType.Bit)).Value = request.HasDepression ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pApplicationId", SqlDbType.Int)).Value = request.ApplicationId;
                    cmd.Parameters.Add(new SqlParameter("@pSelectedServiceType", SqlDbType.VarChar)).Value = DbHelper.NullString(member.SelectedServiceType);

                    cmd.Parameters.Add(new SqlParameter("@pPreliminary_Visits_Requested", SqlDbType.Int)).Value = request.NumberOfVisits;

                    cmd.Parameters.Add(new SqlParameter("@pOther_Injury", SqlDbType.Bit)).Value = request.OtherInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPost_Surgery", SqlDbType.Bit)).Value = request.PostSurgery ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pNo_Injury", SqlDbType.Bit)).Value = request.NoInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDuration", SqlDbType.VarChar)).Value = request.Duration;
                    cmd.Parameters.Add(new SqlParameter("@pLimit_Communication", SqlDbType.Bit)).Value = request.LimitCommunication ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPain_History", SqlDbType.Bit)).Value = request.HasPainHistory ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPSFS_Score", SqlDbType.Decimal)).Value = (request.PsfsScore == -1m) ? (object)DBNull.Value : request.PsfsScore;
                    cmd.Parameters.Add(new SqlParameter("@pCondition", SqlDbType.Int)).Value = DbHelper.NullInt(request.Condition);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertAuthLogForPtot(PtotAuthRequest request, Member member, AuthResponse response)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                // Manipulate Request Data 
                var newOrRecurringInjury = 0;
                if (request.Condition.Equals("2"))
                    newOrRecurringInjury = 1;

                var treatedBy = 0;
                if (request.HasReferral)
                    treatedBy = 1;

                //var coTreated = 0;
                //if (request.IsCoTreat)
                //    coTreated = 1;

                var sql =
                    "INSERT INTO Auth_Log(Auth_Number, Auth_Result, Auth_Date, IVR_Code, Member_ID, Member_Seq," + Environment.NewLine +
                    "Auth_Level, Visits_Provided, Visits_Requested, " + Environment.NewLine +
                    "Weeks_Required, Diag1, Diag2, Diag3, NewOrRecurring, " + Environment.NewLine +
                    "RadiatingPain, TreatedBy, Visits_Allowed, Visits_Approved, " + Environment.NewLine +
                    "Days_Allowed, Days_Approved, Score, Severity_Code, Previous_Auths, OnsetDatePoints, Diabetes, " + Environment.NewLine +
                    "Cancer, Obesity, Smoker, Initial_Injury_Visit_Date, Previous_Auth_Num, " + Environment.NewLine +
                    "Patient_Insurance_Id, Care_Plan_Auth_Entry, Requested_From, " + Environment.NewLine +
                    "Ordering_Provider_Name, Ordering_Provider_Address, Ordering_Provider_City, Ordering_Provider_State, Ordering_Provider_Zip_Code, " + Environment.NewLine +
                    "Ordering_Provider_Phone, Ordering_Provider_Fax, Ordering_Provider_Diagnosis, Referral_Date, " + Environment.NewLine +
                    "Other_Injury, Post_Surgery, No_Injury, Duration, Prior_Treatment, " + Environment.NewLine +
                    "Upper_Extremity, Lower_Extremity, LS_Spine, CT_Spine, Hand_Wrist, Other_Body_Region, " + Environment.NewLine +
                    "PSFS_Score, Pain_History, Drinker, Opioids, Exercise, Confident, " + Environment.NewLine +
                    "Depression, Limit_Communication, CNS, Cardiovascular_Condition, Lung_Disease, ApplicationId, SelectedServiceType, Preliminary_Visits_Requested, Condition) " + Environment.NewLine + 
                    "VALUES(@pAuth_Num, @iAuth_code, getdate(), @pIvr_code, @pSubscriber_Id, @pMember_Seq, " + Environment.NewLine +
                    "@pAuth_request_type, @pPrior_treatment_visits, @pVisits_Requested, " + Environment.NewLine +
                    "@pWeeks_Requested, @pICD9_code_1, @pICD9_code_2, @pICD9_code_3, @iNew_or_recurring_injury, " + Environment.NewLine +
                    "@pPain_Rating, @iTreatedBy, @pVisits_Allowed, @iVisitsApproved, " + Environment.NewLine +
                    "@pDays_Allowed,  @iDays_Allowed, @pScore, @pSevereCode, @pPreviousAuths, @pOnSetDatePoints, @pDiabetes, " + Environment.NewLine +
                    "@pCancer, @pObesity, @pSmoker, @iInitial_Visit_date, @pPrior_Auth_number, " + Environment.NewLine +
                    "@pPatient_Id, @pPlan_Auth_Entry, @pStart_Date_Requested, " + Environment.NewLine +
                    "@pOrdering_Provider_Name, @pOrdering_Provider_Address, @pOrdering_Provider_City, @pOrdering_Provider_State, @pOrdering_Provider_Zip_Code, " + Environment.NewLine +
                    "@pOrdering_Provider_Phone, @pOrdering_Provider_Fax, @pOrdering_Provider_Diagnosis, @pReferral_Date, " + Environment.NewLine +
                    "@pOther_Injury, @pPost_Surgery, @pNo_Injury, @pDuration, @pPrior_Treatment, " + Environment.NewLine +
                    "@pUpper_Extremity, @pLower_Extremity, @pLS_Spine, @pCT_Spine, @pHand_Wrist, @pOther_Body_Region, " + Environment.NewLine +
                    "@pPSFS_Score, @pPain_History, @pDrinker, @pOpioids, @pExercise, @pConfident, " + Environment.NewLine +
                    "@pDepression, @pLimit_Communication, @pCNS, @pCardiovascular_Condition, @pLung_Disease, @pApplicationId, @pSelectedServiceType, @pPreliminary_Visits_Requested, @pCondition)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_Num", SqlDbType.VarChar)).Value = response.AuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@iAuth_code", SqlDbType.VarChar)).Value = response.AuthCode;
                    cmd.Parameters.Add(new SqlParameter("@pIvr_code", SqlDbType.VarChar)).Value = request.IvrCode;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_Seq", SqlDbType.VarChar)).Value = member.MemberSeq;

                    // old codes use request.RequestType ???
                    cmd.Parameters.Add(new SqlParameter("@pAuth_request_type", SqlDbType.Int)).Value = member.NewOrExtended;
                    //cmd.Parameters.Add(new SqlParameter("@pInitial_Injury_date", SqlDbType.DateTime)).Value = request.InitialInjuryDate;
                    cmd.Parameters.Add(new SqlParameter("@pPrior_treatment_visits", SqlDbType.Int)).Value = request.ActualVisits;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Requested", SqlDbType.Int)).Value = member.VisitsRequested;

                    cmd.Parameters.Add(new SqlParameter("@pWeeks_Requested", SqlDbType.Int)).Value = request.NumberOfWeeks;
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_1", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_2", SqlDbType.VarChar)).Value = request.SecondaryDiagnosisCode.ToUpper();
                    cmd.Parameters.Add(new SqlParameter("@pICD9_code_3", SqlDbType.VarChar)).Value = string.Empty;
                    cmd.Parameters.Add(new SqlParameter("@iNew_or_recurring_injury", SqlDbType.Int)).Value = newOrRecurringInjury;
                    //cmd.Parameters.Add(new SqlParameter("@pDaily_Living_Rating", SqlDbType.Int)).Value = request.DailyLivingRating;

                    cmd.Parameters.Add(new SqlParameter("@pPain_Rating", SqlDbType.Int)).Value = request.PainRating;
                    //cmd.Parameters.Add(new SqlParameter("@pRange_of_Motion_Rating", SqlDbType.Int)).Value = request.RangeOfMotionRating;
                    cmd.Parameters.Add(new SqlParameter("@iTreatedBy", SqlDbType.Int)).Value = treatedBy;
                    cmd.Parameters.Add(new SqlParameter("@pVisits_Allowed", SqlDbType.Int)).Value = member.AllowedVisits;
                    cmd.Parameters.Add(new SqlParameter("@iVisitsApproved", SqlDbType.Int)).Value = response.VisitsApproved;

                    cmd.Parameters.Add(new SqlParameter("@pDays_Allowed", SqlDbType.Int)).Value = member.AllowedDays;
                    // old codes use 0 which is definately wrong ???
                    cmd.Parameters.Add(new SqlParameter("@iDays_Allowed", SqlDbType.Int)).Value = member.ApprovedDays;
                    cmd.Parameters.Add(new SqlParameter("@pScore", SqlDbType.Int)).Value = member.CurrentScore;
                    cmd.Parameters.Add(new SqlParameter("@pSevereCode", SqlDbType.VarChar)).Value = member.SevereCode;
                    cmd.Parameters.Add(new SqlParameter("@pPreviousAuths", SqlDbType.Int)).Value = member.NumberOfPreviousAuths;
                    cmd.Parameters.Add(new SqlParameter("@pOnSetDatePoints", SqlDbType.Int)).Value = member.OnSetDatePoints;
                    cmd.Parameters.Add(new SqlParameter("@pDiabetes", SqlDbType.Bit)).Value = request.HasDiabetes ? 1 : 0;
                    //cmd.Parameters.Add(new SqlParameter("@pStroke", SqlDbType.Bit)).Value = request.HasStroke ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pCancer", SqlDbType.Bit)).Value = request.HasCancer ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pObesity", SqlDbType.Bit)).Value = request.IsOverweight ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pSmoker", SqlDbType.Bit)).Value = request.IsSmoker ? 1 : 0;
                    //cmd.Parameters.Add(new SqlParameter("@pChronic_pain", SqlDbType.Bit)).Value = request.HasChronicPain ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@iInitial_Visit_date", SqlDbType.DateTime)).Value = request.InitialVisitDate;
                    //cmd.Parameters.Add(new SqlParameter("@iCo_Treated", SqlDbType.Bit)).Value = coTreated;
                    cmd.Parameters.Add(new SqlParameter("@pPrior_Auth_number", SqlDbType.VarChar)).Value = string.Empty;

                    cmd.Parameters.Add(new SqlParameter("@pPatient_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_Auth_Entry", SqlDbType.VarChar)).Value = request.RequestType;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;

                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Name", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderName);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Address", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderAddress);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_City", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderCity);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_State", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderState);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Zip_Code", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderZipCode);

                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Phone", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderPhone);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Fax", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderFax);
                    cmd.Parameters.Add(new SqlParameter("@pOrdering_Provider_Diagnosis", SqlDbType.VarChar)).Value = DbHelper.NullString(request.OrderingProviderDiagnosis);
                    cmd.Parameters.Add(new SqlParameter("@pReferral_Date", SqlDbType.DateTime)).Value = DbHelper.NullDateTime(request.ReferralDate);

                    cmd.Parameters.Add(new SqlParameter("@pOther_Injury", SqlDbType.Bit)).Value = request.OtherInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pPost_Surgery", SqlDbType.Bit)).Value = request.PostSurgery ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pNo_Injury", SqlDbType.Bit)).Value = request.NoInjury ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDuration", SqlDbType.VarChar)).Value = request.Duration;
                    cmd.Parameters.Add(new SqlParameter("@pPrior_Treatment", SqlDbType.VarChar)).Value = request.PriorTreatment;

                    cmd.Parameters.Add(new SqlParameter("@pUpper_Extremity", SqlDbType.Bit)).Value = request.UpperExtremity ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLower_Extremity", SqlDbType.Bit)).Value = request.LowerExtremity ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLS_Spine", SqlDbType.Bit)).Value = request.LsSpine ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pCT_Spine", SqlDbType.Bit)).Value = request.CtSpine ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pHand_Wrist", SqlDbType.Bit)).Value = request.HandWrist ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pOther_Body_Region", SqlDbType.Bit)).Value = request.OtherRegion ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pPSFS_Score", SqlDbType.Decimal)).Value = (request.PsfsScore == -1m) ? (object)DBNull.Value : request.PsfsScore;
                    cmd.Parameters.Add(new SqlParameter("@pPain_History", SqlDbType.Bit)).Value = request.HasPainHistory ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pDrinker", SqlDbType.Bit)).Value = request.IsDrinker ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pOpioids", SqlDbType.Bit)).Value = request.TakeOpioids ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pExercise", SqlDbType.Bit)).Value = request.HasExercise ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pConfident", SqlDbType.Bit)).Value = request.HasConfidence ? 1 : 0;

                    cmd.Parameters.Add(new SqlParameter("@pDepression", SqlDbType.Bit)).Value = request.HasDepression ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLimit_Communication", SqlDbType.Bit)).Value = request.LimitCommunication ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pCNS", SqlDbType.Bit)).Value = request.HasCns ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pCardiovascular_Condition", SqlDbType.Bit)).Value = request.HasCardiovascular ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pLung_Disease", SqlDbType.Bit)).Value = request.HasLungDisease ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pApplicationId", SqlDbType.Int)).Value = request.ApplicationId;
                    cmd.Parameters.Add(new SqlParameter("@pSelectedServiceType", SqlDbType.VarChar)).Value = DbHelper.NullString(member.SelectedServiceType);

                    cmd.Parameters.Add(new SqlParameter("@pPreliminary_Visits_Requested", SqlDbType.Int)).Value = request.NumberOfVisits;
                    cmd.Parameters.Add(new SqlParameter("@pCondition", SqlDbType.Int)).Value = DbHelper.NullInt(request.Condition);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void InsertAuthEvent(int authNumber, string caseNumber)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "if not exists (select * from AuthEvent where case_number='" + caseNumber +
                          "' and Event_Type_Code='RCVDPCP') " + Environment.NewLine +
                          "INSERT INTO AuthEvent(Auth_Number, Case_number, event_type_code, Event_status, " +
                          "entry_user, entry_date, update_user, update_date) " +
                          "VALUES('" + authNumber + "', '" + caseNumber + "', 'PCPR', 'c', '" +
                          Environment.MachineName + "', getdate(), '" + Environment.MachineName + "', getdate())";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static AuthResultResponse GetAuthResult(string authNumber)
        {
            var result = new AuthResultResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    const string sql = "select * from Auth where Auth_Number=@authnum";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@authnum", SqlDbType.VarChar)).Value = authNumber;

                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                result.IsValid = true;
                                result.ErrorMessage = string.Empty;

                                result.AuthNumber = DbHelper.NullString(dr[dr.GetOrdinal("Auth_Number")]);
                                result.AuthTypeId = DbHelper.NullString(dr[dr.GetOrdinal("AuthType_ID")]);
                                result.SubscriberId = DbHelper.NullString(dr[dr.GetOrdinal("Subscriber_ID")]);
                                result.MemberSeq = DbHelper.NullString(dr[dr.GetOrdinal("Member_Seq")]);
                                result.GroupId = DbHelper.NullString(dr[dr.GetOrdinal("Group_ID")]);
                                result.DivisionId = DbHelper.NullString(dr[dr.GetOrdinal("Division_ID")]);
                                result.GroupCoverageStart = DbHelper.NullDateTime(dr[dr.GetOrdinal("Group_Coverage_Start")]);
                                result.NetworkId = DbHelper.NullString(dr[dr.GetOrdinal("Network_ID")]);
                                result.ProviderId = DbHelper.NullString(dr[dr.GetOrdinal("Provider_ID")]);
                                result.ProviderLocationSeq = DbHelper.NullString(dr[dr.GetOrdinal("ProviderLocation_Seq")]);
                                result.ReferredById = DbHelper.NullString(dr[dr.GetOrdinal("Referred_By_ID")]);
                                result.ReferredByName = DbHelper.NullString(dr[dr.GetOrdinal("Referred_By_Name")]);
                                result.CaseIndexRequested = DbHelper.NullString(dr[dr.GetOrdinal("Case_Index_Requested")]);
                                result.VisitsRequested = DbHelper.NullInt(dr[dr.GetOrdinal("Visits_Requested")]);
                                result.PreviousTreatment = DbHelper.NullString(dr[dr.GetOrdinal("Previous_Treatment")]).ToUpper() == "TRUE";
                                result.OtherInsurance = DbHelper.NullString(dr[dr.GetOrdinal("Other_Insurance")]);
                                result.TreatmentType = DbHelper.NullString(dr[dr.GetOrdinal("Treatment_Type")]);
                                result.DiagCode1 = DbHelper.NullString(dr[dr.GetOrdinal("Diagnostic_Code1")]);
                                result.DiagCode2 = DbHelper.NullString(dr[dr.GetOrdinal("Diagnostic_Code2")]);
                                result.NewOrExtended = DbHelper.NullString(dr[dr.GetOrdinal("New_Or_Extended")]).ToUpper() == "TRUE";
                                result.CaseIndexApproved = DbHelper.NullString(dr[dr.GetOrdinal("Case_Index_Approved")]);
                                result.VisitsApproved = DbHelper.NullInt(dr[dr.GetOrdinal("Visits_Approved")]);
                                result.ApprovedFrom = DbHelper.NullDateTime(dr[dr.GetOrdinal("Approved_From")]);
                                result.ApprovedThru = DbHelper.NullDateTime(dr[dr.GetOrdinal("Approved_Thru")]);

                                result.AmountApproved = DbHelper.NullDecimal(dr[dr.GetOrdinal("Amount_Approved")]);
                                result.AuthCode = DbHelper.NullString(dr[dr.GetOrdinal("Auth_Code")]);
                                result.ApprovedBy = DbHelper.NullString(dr[dr.GetOrdinal("Approved_By")]);
                                result.VisitsActual = DbHelper.NullInt(dr[dr.GetOrdinal("Visits_Actual")]);
                                result.AmountActual = DbHelper.NullInt(dr[dr.GetOrdinal("Amount_Actual")]);
                                result.FriScore = DbHelper.NullInt(dr[dr.GetOrdinal("FRI_Score")]);
                                result.Notes = DbHelper.NullString(dr[dr.GetOrdinal("Notes")]);
                                result.EntryDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Entry_Date")]);
                                result.EntryUser = DbHelper.NullString(dr[dr.GetOrdinal("Entry_User")]);
                                result.UpdateDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Update_Date")]);
                                result.UpdateUser = DbHelper.NullString(dr[dr.GetOrdinal("Update_User")]);
                                result.CaseNumber = DbHelper.NullString(dr[dr.GetOrdinal("Case_Number")]);
                                result.DiagCode3 = DbHelper.NullString(dr[dr.GetOrdinal("Diagnostic_3")]);
                                result.DiagCode4 = DbHelper.NullString(dr[dr.GetOrdinal("Diagnositc_4")]);
                                result.Status = DbHelper.NullString(dr[dr.GetOrdinal("Status")]);
                                result.Closed = DbHelper.NullString(dr[dr.GetOrdinal("Closed")]).ToUpper() == "TRUE";
                                result.CaseIndexActual = DbHelper.NullString(dr[dr.GetOrdinal("Case_Index_Actual")]);
                                result.AmountRequested = DbHelper.NullDecimal(dr[dr.GetOrdinal("Amount_Requested")]);
                                result.DrgCategory = DbHelper.NullString(dr[dr.GetOrdinal("DRG_Category")]);
                                result.DrgCode = DbHelper.NullString(dr[dr.GetOrdinal("DRG_Code")]);
                                result.RequestedFrom = DbHelper.NullDateTime(dr[dr.GetOrdinal("Requested_From")]);
                                result.RequestedThru = DbHelper.NullDateTime(dr[dr.GetOrdinal("Requested_Thru")]);
                                result.ApprovedDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Approved_Date")]);
                                result.Narrative = DbHelper.NullString(dr[dr.GetOrdinal("Narrative")]);

                                result.WorkRelated = DbHelper.NullString(dr[dr.GetOrdinal("Work_Related")]);
                                result.AutoAccident = DbHelper.NullString(dr[dr.GetOrdinal("Auto_Accident")]);
                                result.AuditFlag = DbHelper.NullString(dr[dr.GetOrdinal("Audit_Flag")]);
                                result.InjuryDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Injury_Date")]);
                                result.Region = DbHelper.NullString(dr[dr.GetOrdinal("Region")]);
                                result.TplanPrinted = DbHelper.NullString(dr[dr.GetOrdinal("TPlan_Printed")]);
                                result.TplanPrintedDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("TPlan_Printed_Date")]);
                                result.TplanPrintedUser = DbHelper.NullString(dr[dr.GetOrdinal("TPlan_Printed_User")]);
                                result.VisitsToDate = DbHelper.NullInt(dr[dr.GetOrdinal("Visits_To_Date")]);
                                result.AdditionalVisits = DbHelper.NullInt(dr[dr.GetOrdinal("Additional_Visits")]);
                                result.PlanId = DbHelper.NullString(dr[dr.GetOrdinal("Plan_ID")]);
                                result.IvrFlag = DbHelper.NullString(dr[dr.GetOrdinal("IVR_Flag")]);
                                result.Score = DbHelper.NullInt(dr[dr.GetOrdinal("Score")]);
                                result.FaxDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Fax_Date")]);
                                result.FaxUser = DbHelper.NullString(dr[dr.GetOrdinal("Fax_User")]);
                                result.FaxId = DbHelper.NullString(dr[dr.GetOrdinal("Fax_ID")]);
                                result.RecordsReceivedDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Records_Received_Date")]);
                                result.DenialLetterPrinted = DbHelper.NullString(dr[dr.GetOrdinal("denial_letter_printed")]);
                                result.DenialLetterDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("denial_letter_date")]);
                                result.DenialLetterUser = DbHelper.NullString(dr[dr.GetOrdinal("denial_letter_user")]);
                                result.Category = DbHelper.NullString(dr[dr.GetOrdinal("Category")]);
                                result.ExtensionGranted = DbHelper.NullString(dr[dr.GetOrdinal("Extension_Granted")]).ToUpper() == "TRUE";
                                result.HasDiabetes = DbHelper.NullString(dr[dr.GetOrdinal("Diabetes")]).ToUpper() == "TRUE";
                                result.HasStroke = DbHelper.NullString(dr[dr.GetOrdinal("Stroke")]).ToUpper() == "TRUE";

                                result.HasCancer = DbHelper.NullString(dr[dr.GetOrdinal("Cancer")]).ToUpper() == "TRUE";
                                result.IsOverweight = DbHelper.NullString(dr[dr.GetOrdinal("Obesity")]).ToUpper() == "TRUE";
                                result.IsSmoker = DbHelper.NullString(dr[dr.GetOrdinal("Smoker")]).ToUpper() == "TRUE";
                                result.HasChronicPain = DbHelper.NullString(dr[dr.GetOrdinal("Chronic_Pain")]).ToUpper() == "TRUE";
                                result.EdiAuthNumber = DbHelper.NullString(dr[dr.GetOrdinal("EDI_Auth_Number")]);
                                result.BcbsmaNumber = DbHelper.NullString(dr[dr.GetOrdinal("BCBSMA_number")]);
                                result.EdiCtrlNumber = DbHelper.NullString(dr[dr.GetOrdinal("EDI_CTRL_Number")]);
                                result.InUseBy = DbHelper.NullString(dr[dr.GetOrdinal("In_Use_By")]);

                                result.OrderingProviderName = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_Name")]);
                                result.OrderingProviderAddress = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_Address")]);
                                result.OrderingProviderCity = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_City")]);
                                result.OrderingProviderState = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_State")]);
                                result.OrderingProviderZipCode = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_Zip_Code")]);
                                result.OrderingProviderPhone = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_Phone")]);
                                result.OrderingProviderFax = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_Fax")]);
                                result.OrderingProviderDiagnosis = DbHelper.NullString(dr[dr.GetOrdinal("Ordering_Provider_Diagnosis")]);
                                result.ReferralDate = DbHelper.NullDateTime(dr[dr.GetOrdinal("Referral_Date")]);

                                result.PostSurgery = DbHelper.NullString(dr[dr.GetOrdinal("Post_Surgery")]).ToUpper() == "TRUE";
                                result.OtherInjury = DbHelper.NullString(dr[dr.GetOrdinal("Other_Injury")]).ToUpper() == "TRUE";
                                result.NoInjury = DbHelper.NullString(dr[dr.GetOrdinal("No_Injury")]).ToUpper() == "TRUE";
                                result.Duration = DbHelper.NullString(dr[dr.GetOrdinal("Duration")]);
                                result.PriorTreatment = DbHelper.NullString(dr[dr.GetOrdinal("Prior_Treatment")]);

                                result.UpperExtremity = DbHelper.NullString(dr[dr.GetOrdinal("Upper_Extremity")]).ToUpper() == "TRUE";
                                result.LowerExtremity = DbHelper.NullString(dr[dr.GetOrdinal("Lower_Extremity")]).ToUpper() == "TRUE";
                                result.LsSpine = DbHelper.NullString(dr[dr.GetOrdinal("LS_Spine")]).ToUpper() == "TRUE";
                                result.CtSpine = DbHelper.NullString(dr[dr.GetOrdinal("CT_Spine")]).ToUpper() == "TRUE";
                                result.HandWrist = DbHelper.NullString(dr[dr.GetOrdinal("Hand_Wrist")]).ToUpper() == "TRUE";
                                result.OtherRegion = DbHelper.NullString(dr[dr.GetOrdinal("Other_Body_Region")]).ToUpper() == "TRUE";

                                result.PsfsScore = DbHelper.NullDecimal(dr[dr.GetOrdinal("PSFS_Score")]);
                                result.HasPainHistory = DbHelper.NullString(dr[dr.GetOrdinal("Pain_History")]).ToUpper() == "TRUE";
                                result.IsDrinker = DbHelper.NullString(dr[dr.GetOrdinal("Drinker")]).ToUpper() == "TRUE";
                                result.TakeOpioids = DbHelper.NullString(dr[dr.GetOrdinal("Opioids")]).ToUpper() == "TRUE";
                                result.HasExercise = DbHelper.NullString(dr[dr.GetOrdinal("Exercise")]).ToUpper() == "TRUE";
                                result.HasConfidence = DbHelper.NullString(dr[dr.GetOrdinal("Confident")]).ToUpper() == "TRUE";
                                result.HasDepression = DbHelper.NullString(dr[dr.GetOrdinal("Depression")]).ToUpper() == "TRUE";
                                result.LimitCommunication = DbHelper.NullString(dr[dr.GetOrdinal("Limit_Communication")]).ToUpper() == "TRUE";
                                result.HasCns = DbHelper.NullString(dr[dr.GetOrdinal("CNS")]).ToUpper() == "TRUE";
                                result.HasCardiovascular = DbHelper.NullString(dr[dr.GetOrdinal("Cardiovascular_Condition")]).ToUpper() == "TRUE";
                                result.HasLungDisease = DbHelper.NullString(dr[dr.GetOrdinal("Lung_Disease")]).ToUpper() == "TRUE";

                            }
                            else
                            {
                                result.IsValid = false;
                                result.ErrorMessage = "No record exists for Auth Number = " + authNumber + ".";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error getting auth result due to: " + ex.Message;
            }

            return result;
        }

        public static bool IsValidBcbsmaNumber(string bcbsmaNumber, string providerId, 
                                string subscriberId, string memberSeq)
        {
            var result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as bcbsma_count from Auth " +
                                   "where BCBSMA_number=@BCBSMA_number and Provider_ID=@Provider_ID " + 
                                   "and Subscriber_ID=@Subscriber_ID and Member_Seq=@Member_Seq";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@BCBSMA_number", SqlDbType.VarChar)).Value = bcbsmaNumber;
                    cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;
                    cmd.Parameters.Add(new SqlParameter("@Subscriber_ID", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@Member_Seq", SqlDbType.VarChar)).Value = memberSeq;

                    conn.Open();
                    var bcbsmaCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (bcbsmaCount > 0)
                        result = true;
                }
            }

            return result;
        }

        public static bool InsertAuthSsoLog(int authNumber, AuthSsoData ssoData)
        {
            var result = true;

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    var sql = "insert into Auth_Log_Sso (Auth_Number, OrderingPhysicianID, OrderingPhysicianNPI, OrderingVendorID, OrderingVendorNPI, " + Environment.NewLine +
                        "PerformingPhysicianID, PerformingPhysicianNPI, PerformingVendorID, PerformingVendorNPI, FacilityID, FacilityNPI, Service) " + Environment.NewLine +
                        "VALUES (@pAuth_Number, @pOrderingPhysicianID, @pOrderingPhysicianNPI, @pOrderingVendorID, @pOrderingVendorNPI, " + Environment.NewLine +
                        "@pPerformingPhysicianID, @pPerformingPhysicianNPI, @pPerformingVendorID, @pPerformingVendorNPI, @pFacilityID, @pFacilityNPI, @pService)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@pAuth_Number", SqlDbType.VarChar)).Value = authNumber;
                        cmd.Parameters.Add(new SqlParameter("@pOrderingPhysicianID", SqlDbType.VarChar)).Value = ssoData.OrderingPhysicianId;
                        cmd.Parameters.Add(new SqlParameter("@pOrderingPhysicianNPI", SqlDbType.VarChar)).Value = ssoData.OrderingPhysicianNpi;
                        cmd.Parameters.Add(new SqlParameter("@pOrderingVendorID", SqlDbType.VarChar)).Value = ssoData.OrderingVendorId;
                        cmd.Parameters.Add(new SqlParameter("@pOrderingVendorNPI", SqlDbType.VarChar)).Value = ssoData.OrderingVendorNpi;
                        cmd.Parameters.Add(new SqlParameter("@pPerformingPhysicianID", SqlDbType.VarChar)).Value = ssoData.PerformingPhysicianId;
                        cmd.Parameters.Add(new SqlParameter("@pPerformingPhysicianNPI", SqlDbType.VarChar)).Value = ssoData.PerformingPhysicianNpi;
                        cmd.Parameters.Add(new SqlParameter("@pPerformingVendorID", SqlDbType.VarChar)).Value = ssoData.PerformingVendorId;
                        cmd.Parameters.Add(new SqlParameter("@pPerformingVendorNPI", SqlDbType.VarChar)).Value = ssoData.PerformingVendorNpi;
                        cmd.Parameters.Add(new SqlParameter("@pFacilityID", SqlDbType.VarChar)).Value = ssoData.FacilityId;
                        cmd.Parameters.Add(new SqlParameter("@pFacilityNPI", SqlDbType.VarChar)).Value = ssoData.FacilityNpi;
                        cmd.Parameters.Add(new SqlParameter("@pService", SqlDbType.VarChar)).Value = ssoData.Service;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static bool CheckDuplicateClientAuthNumber(string clientAuthNumber, string subscriberId, string providerId)
        {
            var result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select count(*) as auth_count from Auth " + Environment.NewLine +
                    "where ClientAuthNumber=@pClientAuthNumber " + Environment.NewLine +
                    "and Subscriber_ID=@pSubscriber_ID " + Environment.NewLine +
                    "and Provider_ID=@pProvider_ID and Status<>'V'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pClientAuthNumber", SqlDbType.VarChar)).Value = clientAuthNumber;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_ID", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_ID", SqlDbType.VarChar)).Value = providerId;

                    conn.Open();
                    var authCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (authCount > 0)
                    {
                        result = true;                            
                    }
                }
            }

            return result;            
        }
    }
}
