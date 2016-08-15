using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RRS.BEL;

namespace RRS.DAL
{
    public class MemberAdapter
    {
        public static Dictionary<string, string> CheckMember(string code, bool isIvr, string memberId,
                                                             DateTime dateOfBirth, DateTime startDate)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                if (isIvr)
                {
                    using (var cmd = new SqlCommand("rrs_Validate_Member_IVR", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var ivrCode = cmd.Parameters.Add("@ivr_code", SqlDbType.VarChar, 10);
                        ivrCode.Value = code;
                        ivrCode.Direction = ParameterDirection.Input;
                        var mId = cmd.Parameters.Add("@member_id", SqlDbType.VarChar, 22);
                        mId.Value = memberId;
                        mId.Direction = ParameterDirection.Input;
                        
                        var dob = cmd.Parameters.Add("@dob", SqlDbType.DateTime, 8);
                        dob.Direction = ParameterDirection.Output;

                        var sDate = cmd.Parameters.Add("@requestedStartDate", SqlDbType.DateTime, 8);
                        sDate.Value = startDate;
                        sDate.Direction = ParameterDirection.Input;

                        //output
                        var errNum = cmd.Parameters.Add("@err_num", SqlDbType.Int, 4);
                        errNum.Direction = ParameterDirection.Output;
                        var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                        errMsg.Direction = ParameterDirection.Output;
                        var memberName = cmd.Parameters.Add("@member_name", SqlDbType.VarChar, 50);
                        memberName.Direction = ParameterDirection.Output;
                        var planMaxVisits = cmd.Parameters.Add("@plan_max_visits", SqlDbType.Int, 4);
                        planMaxVisits.Direction = ParameterDirection.Output;
                        var actualVisits = cmd.Parameters.Add("@actual_visits_for_member", SqlDbType.Int, 4);
                        actualVisits.Direction = ParameterDirection.Output;
                        var subscriberId = cmd.Parameters.Add("@subscriber_ID", SqlDbType.VarChar, 20);
                        subscriberId.Direction = ParameterDirection.Output;
                        var memberSeq = cmd.Parameters.Add("@MemberSeq", SqlDbType.VarChar, 2);
                        memberSeq.Direction = ParameterDirection.Output;
                        var planId = cmd.Parameters.Add("@planid", SqlDbType.VarChar, 20);
                        planId.Direction = ParameterDirection.Output;
                        var actualStartDate = cmd.Parameters.Add("@pActual_visits_start_date", SqlDbType.DateTime, 8);
                        actualStartDate.Direction = ParameterDirection.Output;
                        var actualEndDate = cmd.Parameters.Add("@pActual_visits_end_date", SqlDbType.DateTime, 8);
                        actualEndDate.Direction = ParameterDirection.Output;
                        var visitYearType = cmd.Parameters.Add("@pVisit_Year_Type", SqlDbType.Char, 1);
                        visitYearType.Direction = ParameterDirection.Output;
                        var unmanagedVisits = cmd.Parameters.Add("@Unmanaged_Visits", SqlDbType.Int, 4);
                        unmanagedVisits.Direction = ParameterDirection.Output;
                        var claimsAddressError = cmd.Parameters.Add("@claimsAddressError", SqlDbType.VarChar, 500);
                        claimsAddressError.Direction = ParameterDirection.Output;
                        var claimsAddressId = cmd.Parameters.Add("@claimsAddressId", SqlDbType.Int, 4);
                        claimsAddressId.Direction = ParameterDirection.Output;
                        var claimsAddress = cmd.Parameters.Add("@claimsAddress", SqlDbType.VarChar, 200);
                        claimsAddress.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        dict["err_num"] = DbHelper.NullString(errNum.Value);
                        dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                        dict["member_name"] = DbHelper.NullString(memberName.Value);
                        dict["plan_max_visits"] = DbHelper.NullString(planMaxVisits.Value);
                        dict["actual_visits"] = DbHelper.NullString(actualVisits.Value);
                        dict["subscriber_id"] = DbHelper.NullString(subscriberId.Value);
                        dict["member_seq"] = DbHelper.NullString(memberSeq.Value);
                        dict["plan_id"] = DbHelper.NullString(planId.Value);
                        dict["actual_start_date"] = DbHelper.NullString(actualStartDate.Value);
                        dict["actual_end_date"] = DbHelper.NullString(actualEndDate.Value);
                        dict["visit_year_type"] = DbHelper.NullString(visitYearType.Value);
                        dict["unmanaged_visits"] = DbHelper.NullString(unmanagedVisits.Value);
                        dict["claimsAddressError"] = DbHelper.NullString(claimsAddressError.Value);
                        dict["claimsAddressId"] = DbHelper.NullString(claimsAddressId.Value);
                        dict["claimsAddress"] = DbHelper.NullString(claimsAddress.Value);
                        dict["dob"] = DbHelper.NullString(dob.Value);
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand("rrs_Validate_Member", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = code;
                        cmd.Parameters.Add(new SqlParameter("@member_id", SqlDbType.VarChar)).Value = memberId;
                        cmd.Parameters.Add(new SqlParameter("@dob", SqlDbType.DateTime)).Value = dateOfBirth;
                        cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value =
                            startDate;
                        //output
                        var errNum = cmd.Parameters.Add("@err", SqlDbType.Char, 1);
                        errNum.Direction = ParameterDirection.Output;
                        var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                        errMsg.Direction = ParameterDirection.Output;
                        var memberName = cmd.Parameters.Add("@member_name", SqlDbType.VarChar, 50);
                        memberName.Direction = ParameterDirection.Output;
                        var planMaxVisits = cmd.Parameters.Add("@plan_max_visits", SqlDbType.Int, 4);
                        planMaxVisits.Direction = ParameterDirection.Output;
                        var actualVisits = cmd.Parameters.Add("@actual_visits_for_member", SqlDbType.Int, 4);
                        actualVisits.Direction = ParameterDirection.Output;
                        var subscriberId = cmd.Parameters.Add("@subscriber_ID", SqlDbType.VarChar, 20);
                        subscriberId.Direction = ParameterDirection.Output;
                        var memberSeq = cmd.Parameters.Add("@MemberSeq", SqlDbType.VarChar, 2);
                        memberSeq.Direction = ParameterDirection.Output;
                        var planId = cmd.Parameters.Add("@planid", SqlDbType.VarChar, 20);
                        planId.Direction = ParameterDirection.Output;
                        var actualStartDate = cmd.Parameters.Add("@pActual_visits_start_date", SqlDbType.DateTime, 8);
                        actualStartDate.Direction = ParameterDirection.Output;
                        var actualEndDate = cmd.Parameters.Add("@pActual_visits_end_date", SqlDbType.DateTime, 8);
                        actualEndDate.Direction = ParameterDirection.Output;
                        var visitYearType = cmd.Parameters.Add("@pVisit_Year_Type", SqlDbType.Char, 1);
                        visitYearType.Direction = ParameterDirection.Output;
                        var unmanagedVisits = cmd.Parameters.Add("@Unmanaged_Visits", SqlDbType.Int, 4);
                        unmanagedVisits.Direction = ParameterDirection.Output;
                        var claimsAddressError = cmd.Parameters.Add("@claimsAddressError", SqlDbType.VarChar, 500);
                        claimsAddressError.Direction = ParameterDirection.Output;
                        var claimsAddressId = cmd.Parameters.Add("@claimsAddressId", SqlDbType.Int, 4);
                        claimsAddressId.Direction = ParameterDirection.Output;
                        var claimsAddress = cmd.Parameters.Add("@claimsAddress", SqlDbType.VarChar, 200);
                        claimsAddress.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        dict["err_num"] = DbHelper.NullString(errNum.Value);
                        dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                        dict["member_name"] = DbHelper.NullString(memberName.Value);
                        dict["plan_max_visits"] = DbHelper.NullString(planMaxVisits.Value);
                        dict["actual_visits"] = DbHelper.NullString(actualVisits.Value);
                        dict["subscriber_id"] = DbHelper.NullString(subscriberId.Value);
                        dict["member_seq"] = DbHelper.NullString(memberSeq.Value);
                        dict["plan_id"] = DbHelper.NullString(planId.Value);
                        dict["actual_start_date"] = DbHelper.NullString(actualStartDate.Value);
                        dict["actual_end_date"] = DbHelper.NullString(actualEndDate.Value);
                        dict["visit_year_type"] = DbHelper.NullString(visitYearType.Value);
                        dict["unmanaged_visits"] = DbHelper.NullString(unmanagedVisits.Value);
                        dict["claimsAddressError"] = DbHelper.NullString(claimsAddressError.Value);
                        dict["claimsAddressId"] = DbHelper.NullString(claimsAddressId.Value);
                        dict["claimsAddress"] = DbHelper.NullString(claimsAddress.Value);
                        dict["dob"] = dateOfBirth.ToShortDateString();
                    }
                }
            }

            return dict;
        }

        public static Dictionary<string, string> CheckMemberFromSso(string clientPrefix, string subscriberId, string blindKey)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select top 1 Member_Seq " + Environment.NewLine +
                          "from Members " + Environment.NewLine +
                          "where Subscriber_ID=@pSubscriber_ID and Alternate_ID=@pAlternate_ID";

                string memberSeq;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_ID", SqlDbType.VarChar)).Value = clientPrefix + subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pAlternate_ID", SqlDbType.VarChar)).Value = blindKey;
                    conn.Open();

                    memberSeq = DbHelper.NullString(cmd.ExecuteScalar());
                }

                if (memberSeq.Equals(string.Empty))
                {
                    dict["err_num"] = "Y";
                    dict["errormessage"] = "Member not found.";
                    dict["member_seq"] = string.Empty;
                }
                else
                {
                    // WI #12101
                    //dict = CheckMemberFromWeb(code, subscriberId, memberSeq, authTypeId, dateOfBirth, startDate, selectedAuthType);
                    dict["err_num"] = "N";
                    dict["errormessage"] = string.Empty;
                    dict["member_seq"] = memberSeq;
                }
            }

            return dict;
        }


        public static Dictionary<string, string> CheckMemberFromWeb(string code, string subscriberId, string memberSeq, 
                                                    string authTypeId, DateTime dateOfBirth, DateTime startDate)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_Validate_Member_Web", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = code;
                    cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@member_seq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@dob", SqlDbType.DateTime)).Value = dateOfBirth;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = startDate;
                    cmd.Parameters.Add(new SqlParameter("@AuthType_ID", SqlDbType.VarChar)).Value = authTypeId;
                    cmd.Parameters.Add(new SqlParameter("@selectedAuthType", SqlDbType.VarChar)).Value = string.Empty;
                    //output
                    var errNum = cmd.Parameters.Add("@err", SqlDbType.Char, 1);
                    errNum.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                    errMsg.Direction = ParameterDirection.Output;
                    var memberName = cmd.Parameters.Add("@member_name", SqlDbType.VarChar, 50);
                    memberName.Direction = ParameterDirection.Output;
                    var planMaxVisits = cmd.Parameters.Add("@plan_max_visits", SqlDbType.Int, 4);
                    planMaxVisits.Direction = ParameterDirection.Output;
                    var actualVisits = cmd.Parameters.Add("@actual_visits_for_member", SqlDbType.Int, 4);
                    actualVisits.Direction = ParameterDirection.Output;
                    var planId = cmd.Parameters.Add("@planid", SqlDbType.VarChar, 20);
                    planId.Direction = ParameterDirection.Output;
                    var actualStartDate = cmd.Parameters.Add("@pActual_visits_start_date", SqlDbType.DateTime, 8);
                    actualStartDate.Direction = ParameterDirection.Output;
                    var actualEndDate = cmd.Parameters.Add("@pActual_visits_end_date", SqlDbType.DateTime, 8);
                    actualEndDate.Direction = ParameterDirection.Output;
                    var visitYearType = cmd.Parameters.Add("@pVisit_Year_Type", SqlDbType.Char, 1);
                    visitYearType.Direction = ParameterDirection.Output;
                    var unmanagedVisits = cmd.Parameters.Add("@Unmanaged_Visits", SqlDbType.Int, 4);
                    unmanagedVisits.Direction = ParameterDirection.Output;
                    var claimsAddressError = cmd.Parameters.Add("@claimsAddressError", SqlDbType.VarChar, 500);
                    claimsAddressError.Direction = ParameterDirection.Output;
                    var claimsAddressId = cmd.Parameters.Add("@claimsAddressId", SqlDbType.Int, 4);
                    claimsAddressId.Direction = ParameterDirection.Output;
                    var claimsAddress = cmd.Parameters.Add("@claimsAddress", SqlDbType.VarChar, 200);
                    claimsAddress.Direction = ParameterDirection.Output;
                    var isMedicareAdvantage = cmd.Parameters.Add("@isMedicareAdvantage", SqlDbType.Bit, 1);
                    isMedicareAdvantage.Direction = ParameterDirection.Output;
                    var prefixSubscriberId = cmd.Parameters.Add("@prefix_subscriber_id", SqlDbType.VarChar, 20);
                    prefixSubscriberId.Direction = ParameterDirection.Output;
                    var useCareRegistration = cmd.Parameters.Add("@useCareRegistration", SqlDbType.Bit, 1);
                    useCareRegistration.Direction = ParameterDirection.Output;
                    var realAuthTypeId = cmd.Parameters.Add("@realAuthTypeID", SqlDbType.VarChar, 20);
                    realAuthTypeId.Direction = ParameterDirection.Output;


                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["err_num"] = DbHelper.NullString(errNum.Value);
                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                    dict["member_name"] = DbHelper.NullString(memberName.Value);
                    dict["plan_max_visits"] = DbHelper.NullString(planMaxVisits.Value);
                    dict["actual_visits"] = DbHelper.NullString(actualVisits.Value);
                    dict["plan_id"] = DbHelper.NullString(planId.Value);
                    dict["actual_start_date"] = DbHelper.NullString(actualStartDate.Value);
                    dict["actual_end_date"] = DbHelper.NullString(actualEndDate.Value);
                    dict["visit_year_type"] = DbHelper.NullString(visitYearType.Value);
                    dict["unmanaged_visits"] = DbHelper.NullString(unmanagedVisits.Value);
                    dict["claimsAddressError"] = DbHelper.NullString(claimsAddressError.Value);
                    dict["claimsAddressId"] = DbHelper.NullString(claimsAddressId.Value);
                    dict["claimsAddress"] = DbHelper.NullString(claimsAddress.Value);
                    dict["dob"] = dateOfBirth.ToShortDateString();
                    dict["isMedicareAdvantage"] = DbHelper.NullString(isMedicareAdvantage.Value);
                    dict["prefix_subscriber_id"] = DbHelper.NullString(prefixSubscriberId.Value);
                    dict["useCareRegistration"] = DbHelper.NullString(useCareRegistration.Value);
                    dict["realAuthTypeId"] = DbHelper.NullString(realAuthTypeId.Value);
                }
            }

            return dict;
        }

        public static Dictionary<string, string> PreCheckMemeberInfo(string ivrCode, string subscriberId, string memberSeq, DateTime startDate, DateTime dateOfBirth)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_Patient_ID_Pre_Check", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = ivrCode;
                    cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@member_seq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = startDate;
                    cmd.Parameters.Add(new SqlParameter("@dob", SqlDbType.DateTime)).Value = dateOfBirth;
                    //output
                    var errNum = cmd.Parameters.Add("@err", SqlDbType.Char, 1);
                    errNum.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                    errMsg.Direction = ParameterDirection.Output;
                    var prefixSubscriberId = cmd.Parameters.Add("@prefix_subscriber_id", SqlDbType.VarChar, 20);
                    prefixSubscriberId.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["err_num"] = DbHelper.NullString(errNum.Value);
                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                    dict["prefix_subscriber_id"] = DbHelper.NullString(prefixSubscriberId.Value);
                }
            }

            return dict;
        }

        public static DataSet CheckInsuranceId(string ivrCode, string subscriberId, DateTime requestedStartDate)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_Validate_Insurance_ID_Angel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = ivrCode;
                    cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = 
                        requestedStartDate.Equals(new DateTime(1900, 1, 1)) ? (object)DBNull.Value : requestedStartDate;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "InsuranceId");
                    }
                }
            }

            return ds;
        }

        public static Dictionary<string, string> GetAuthTypeFromSelectedServiceType(string prefixSubscriberId, string memberSeq, DateTime startDate, string serviceType)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_GetAuthTypeFromSelectedServiceType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@prefix_subscriber_id", SqlDbType.VarChar)).Value = prefixSubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@member_seq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = startDate;
                    cmd.Parameters.Add(new SqlParameter("@serviceType", SqlDbType.VarChar)).Value = serviceType;
                    //output
                    var errNum = cmd.Parameters.Add("@err", SqlDbType.Char, 1);
                    errNum.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                    errMsg.Direction = ParameterDirection.Output;
                    var authType = cmd.Parameters.Add("@AuthType", SqlDbType.VarChar, 20);
                    authType.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["err_num"] = DbHelper.NullString(errNum.Value);
                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                    dict["auth_type"] = DbHelper.NullString(authType.Value);
                }
            }

            return dict;
        }
        
        public static Member GetMemberInfo(AuthRequest request, string authTypeId)
        {
            var member = new Member {SubscriberId = request.SubscriberId, MemberSeq = request.MemberSeq};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select gn.NetworkID, s.Group_ID, s.Division_ID, s.Plan_ID, s.Group_Coverage_Start, " +
                                   "p.Unmanaged_Visits, p.Total_Visit_Max_Initial, p.Total_Visit_Max_Continuation, p.Care_Registration_Visits, p.Fail_To_PreAuth_Days " +
                                   "from SubEnrollment s inner join GroupNetwork gn on s.Group_ID=gn.Group_ID " + 
                                   "and s.Division_ID=gn.Division_ID and s.Group_Coverage_Start=gn.coverage_start " + 
                                   "inner join Networks n on gn.NetworkID=n.Network_ID " +
                                   "inner join Plans p on s.Plan_ID=p.Plan_ID " +
                                   "where s.Subscriber_ID = @pSubscriber_id and s.Void=0 " +
                                   "and s.Start_Date<=@pStart_Date_Requested " +
                                   "and (s.End_Date IS NULL or s.End_Date>=@pStart_Date_Requested) " +
                                   "and p.AuthType_ID=@pAuthType_id " + 
                                   "ORDER BY ISNULL(s.End_Date, '1/1/2100') DESC, s.Start_Date DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_id", SqlDbType.VarChar)).Value = request.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_id", SqlDbType.VarChar)).Value = authTypeId;

                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            member.GroupId = DbHelper.NullString(dr[dr.GetOrdinal("Group_ID")]);
                            member.DivisionId = DbHelper.NullString(dr[dr.GetOrdinal("Division_ID")]);
                            member.PlanId = DbHelper.NullString(dr[dr.GetOrdinal("Plan_ID")]);
                            member.GroupCoverageStart = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Group_Coverage_Start")]));
                            member.NetworkId = DbHelper.NullString(dr[dr.GetOrdinal("NetworkID")]);
                            member.UnmanagedVisits = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Unmanaged_Visits")]));
                            member.PlanVisitMaxInitial = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Total_Visit_Max_Initial")]));
                            member.PlanVisitMaxContinuation = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Total_Visit_Max_Continuation")]));
                            member.CareRegistrationVisits = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Care_Registration_Visits")]));
                            member.FailToPreAuthDays = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Fail_To_PreAuth_Days")]));
                        }
                    }
                }
            }

            return member;
        }

        public static void GetAllowedVisitsAndDays(Member member, string requestType, string provSuffix,  
                                int mildStart, int mildEnd, int modStart, int modEnd, int severeStart, int severeEnd)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_SetVisitsAndDays_Web", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_request_type", SqlDbType.VarChar)).Value = requestType;
                    cmd.Parameters.Add(new SqlParameter("@pScore", SqlDbType.Int)).Value = member.CurrentScore;
                    cmd.Parameters.Add(new SqlParameter("@pProv_Suffix", SqlDbType.VarChar)).Value = provSuffix;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Code", SqlDbType.VarChar)).Value = member.DrgCode;
                    cmd.Parameters.Add(new SqlParameter("@pMild_Start", SqlDbType.Int)).Value = mildStart;
                    cmd.Parameters.Add(new SqlParameter("@pMild_End", SqlDbType.Int)).Value = mildEnd;
                    cmd.Parameters.Add(new SqlParameter("@pMod_Start", SqlDbType.Int)).Value = modStart;
                    cmd.Parameters.Add(new SqlParameter("@pMod_End", SqlDbType.Int)).Value = modEnd;
                    cmd.Parameters.Add(new SqlParameter("@pSev_Start", SqlDbType.Int)).Value = severeStart;
                    cmd.Parameters.Add(new SqlParameter("@pSev_End", SqlDbType.Int)).Value = severeEnd;

                    //output
                    var visitsAllowed = cmd.Parameters.Add("@pVisits_Allowed", SqlDbType.Int, 4);
                    visitsAllowed.Direction = ParameterDirection.Output;
                    var daysAllowed = cmd.Parameters.Add("@pDays_Allowed", SqlDbType.Int, 4);
                    daysAllowed.Direction = ParameterDirection.Output;
                    var severeCode = cmd.Parameters.Add("@pSevereCode", SqlDbType.Char, 1);
                    severeCode.Direction = ParameterDirection.Output;
                    var acute = cmd.Parameters.Add("@pAcute", SqlDbType.Bit, 1);
                    acute.Direction = ParameterDirection.Output;
                    var requireManagement = cmd.Parameters.Add("@pRequiresManagment", SqlDbType.Bit, 1);
                    requireManagement.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    member.AllowedVisits = int.Parse(DbHelper.NullString(visitsAllowed.Value));
                    member.AllowedDays = int.Parse(DbHelper.NullString(daysAllowed.Value));
                    member.SevereCode = DbHelper.NullString(severeCode.Value);
                }
            }
        }

        public static int GetAllowedVisits(string drgCode, int score, string requestType, string provSuffix,
                                int mildStart, int mildEnd, int modStart, int modEnd, int severeStart, int severeEnd)
        {
            int result = 0;
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_SetVisitsAndDays_Web", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pAuth_request_type", SqlDbType.VarChar)).Value = requestType;
                    cmd.Parameters.Add(new SqlParameter("@pScore", SqlDbType.Int)).Value = score;
                    cmd.Parameters.Add(new SqlParameter("@pProv_Suffix", SqlDbType.VarChar)).Value = provSuffix;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Code", SqlDbType.VarChar)).Value = drgCode;
                    cmd.Parameters.Add(new SqlParameter("@pMild_Start", SqlDbType.Int)).Value = mildStart;
                    cmd.Parameters.Add(new SqlParameter("@pMild_End", SqlDbType.Int)).Value = mildEnd;
                    cmd.Parameters.Add(new SqlParameter("@pMod_Start", SqlDbType.Int)).Value = modStart;
                    cmd.Parameters.Add(new SqlParameter("@pMod_End", SqlDbType.Int)).Value = modEnd;
                    cmd.Parameters.Add(new SqlParameter("@pSev_Start", SqlDbType.Int)).Value = severeStart;
                    cmd.Parameters.Add(new SqlParameter("@pSev_End", SqlDbType.Int)).Value = severeEnd;

                    //output
                    var visitsAllowed = cmd.Parameters.Add("@pVisits_Allowed", SqlDbType.Int, 4);
                    visitsAllowed.Direction = ParameterDirection.Output;
                    var daysAllowed = cmd.Parameters.Add("@pDays_Allowed", SqlDbType.Int, 4);
                    daysAllowed.Direction = ParameterDirection.Output;
                    var severeCode = cmd.Parameters.Add("@pSevereCode", SqlDbType.Char, 1);
                    severeCode.Direction = ParameterDirection.Output;
                    var acute = cmd.Parameters.Add("@pAcute", SqlDbType.Bit, 1);
                    acute.Direction = ParameterDirection.Output;
                    var requireManagement = cmd.Parameters.Add("@pRequiresManagment", SqlDbType.Bit, 1);
                    requireManagement.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    result = int.Parse(DbHelper.NullString(visitsAllowed.Value));
                }
            }

            return result;
        }

        public static void GetLastScores(AuthRequest request, Member member)
        {
            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_GetLastScores", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pDRG_Code", SqlDbType.VarChar)).Value = member.DrgCode;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pLimit_To_Year", SqlDbType.Bit)).Value = member.LimitToYear ? 1 : 0;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_id", SqlDbType.VarChar)).Value = member.AuthTypeId;

                    // output 
                    var lastFriScore = cmd.Parameters.Add("@pLastFriScore", SqlDbType.Int, 4);
                    lastFriScore.Direction = ParameterDirection.Output;
                    var lastPsfsScore = cmd.Parameters.Add("@pLastPsfsScore", SqlDbType.Decimal, 5);
                    lastPsfsScore.Direction = ParameterDirection.Output;
                    lastPsfsScore.Precision = 5;
                    lastPsfsScore.Scale = 1;
                    var lastScore = cmd.Parameters.Add("@pLastScore", SqlDbType.Int, 4);
                    lastScore.Direction = ParameterDirection.Output;
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (member.AuthTypeId.Equals("DC"))
                    {
                        member.LastFriScore = int.Parse(DbHelper.NullString(lastFriScore.Value));
                        member.LastPsfsScore = 0m;
                    }
                    else
                    {
                        member.LastFriScore = 0;
                        member.LastPsfsScore = decimal.Parse(DbHelper.NullString(lastPsfsScore.Value));                        
                    }
                    member.LastScore = int.Parse(DbHelper.NullString(lastScore.Value));
                }
            }
        }

        public static bool FoundLastScore(AuthRequest request, Member member)
        {
            bool found;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select count(*) as last_score_count from AUTH " +
                        "where Subscriber_ID='" + member.SubscriberId + "' " +
                        "and Member_Seq='" + member.MemberSeq + "' " +
                        "and AuthType_ID='" + member.AuthTypeId + "' " + 
                        "and [Status]<>'V' ";

                if (member.GroupId.Contains("BCBSMA_UM"))
                {
                    sql += "and Year(Coalesce(Approved_From, Requested_From))=" + request.StartDate.Year;
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    found = int.Parse(DbHelper.NullString(cmd.ExecuteScalar())) > 0;
                }
            }

            return found;
        }

        public static int GetAutoApprovedVisits(Member member)
        {
            int result;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select isnull(Num_Auto_Approved_visits, 0) from plans " +
                                   "where plan_id=@pPlan_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();
                    result = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                }
            }

            return result;
        }

        public static Dictionary<string, string> CheckMemberPtotAuthType(string code, string subscriberId, string memberSeq,
                                                        DateTime startDate)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_GetRealPTOTAuthType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = code;
                    cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@member_seq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = startDate;
                    //output
                    var errNum = cmd.Parameters.Add("@err", SqlDbType.Char, 1);
                    errNum.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                    errMsg.Direction = ParameterDirection.Output;
                    var authTypeId = cmd.Parameters.Add("@Real_AuthType", SqlDbType.VarChar, 20);
                    authTypeId.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["err_num"] = DbHelper.NullString(errNum.Value);
                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                    dict["authtype_id"] = DbHelper.NullString(authTypeId.Value);
                }
            }

            return dict;
        }

        public static HighmarkMemberResponse GetHighmarkMemberData(string subscriberId, string memberSeq, DateTime inquiryDate)
        {
            var result = new HighmarkMemberResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("[DocGen].[GetHighmarkMemberData]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@subscriberId", SqlDbType.VarChar)).Value = subscriberId;
                        cmd.Parameters.Add(new SqlParameter("@memberSequence", SqlDbType.VarChar)).Value = memberSeq;
                        cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = inquiryDate;

                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                result.IsValid = true;
                                result.ErrorMessage = string.Empty;

                                result.SubscriberId = subscriberId;
                                result.MemberSequence = memberSeq;
                                result.AuthNumber = string.Empty;
                                result.CopcId = DbHelper.NullString(dr[dr.GetOrdinal("CopcId")]);
                                result.CoptC = DbHelper.NullString(dr[dr.GetOrdinal("CoptC")]);
                                result.Eal = DbHelper.NullString(dr[dr.GetOrdinal("EAL")]);
                                result.IsErisa = DbHelper.NullString(dr[dr.GetOrdinal("IsErisa")]);
                                result.GroupNumber = DbHelper.NullString(dr[dr.GetOrdinal("GroupNumber")]);
                                result.GroupName = DbHelper.NullString(dr[dr.GetOrdinal("GroupName")]);
                                result.GroupClientNumber = DbHelper.NullString(dr[dr.GetOrdinal("GroupClientNumber")]);
                                result.PartnerId = DbHelper.NullString(dr[dr.GetOrdinal("PartnerId")]);
                                result.Lob = DbHelper.NullString(dr[dr.GetOrdinal("Lob")]);
                                result.PlanType = DbHelper.NullString(dr[dr.GetOrdinal("PlanType")]);
                            }
                            else
                            {
                                result.IsValid = false;
                                result.ErrorMessage = "No record found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error getting Highmark member data due to: " + ex.Message;                
            }

            return result;
        }

        public static HighmarkMemberResponse GetHighmarkMemberDataByAuth(string authNumber, DateTime inquiryDate)
        {
            var result = new HighmarkMemberResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("[DocGen].[GetHighmarkMemberDataByAuth]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@authNumber", SqlDbType.VarChar)).Value = authNumber;
                        cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = inquiryDate;

                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                result.IsValid = true;
                                result.ErrorMessage = string.Empty;

                                result.SubscriberId = DbHelper.NullString(dr[dr.GetOrdinal("SubscriberId")]);
                                result.MemberSequence = DbHelper.NullString(dr[dr.GetOrdinal("MemberSequence")]);
                                result.AuthNumber = authNumber;
                                result.CopcId = DbHelper.NullString(dr[dr.GetOrdinal("CopcId")]);
                                result.CoptC = DbHelper.NullString(dr[dr.GetOrdinal("CoptC")]);
                                result.Eal = DbHelper.NullString(dr[dr.GetOrdinal("EAL")]);
                                result.IsErisa = DbHelper.NullString(dr[dr.GetOrdinal("IsErisa")]);
                                result.GroupNumber = DbHelper.NullString(dr[dr.GetOrdinal("GroupNumber")]);
                                result.GroupName = DbHelper.NullString(dr[dr.GetOrdinal("GroupName")]);
                                result.GroupClientNumber = DbHelper.NullString(dr[dr.GetOrdinal("GroupClientNumber")]);
                                result.PartnerId = DbHelper.NullString(dr[dr.GetOrdinal("PartnerId")]);
                                result.Lob = DbHelper.NullString(dr[dr.GetOrdinal("Lob")]);
                                result.PlanType = DbHelper.NullString(dr[dr.GetOrdinal("PlanType")]);
                            }
                            else
                            {
                                result.IsValid = false;
                                result.ErrorMessage = "No record found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error getting Highmark member data by auth due to: " + ex.Message;
            }

            return result;
        }

        public static bool IsMedicareAdvantageMember(Member member)
        {
            bool result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select Medicare_Advantage_Special_Handling from Plans where plan_id=@pPlan_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    result = DbHelper.NullString(cmd.ExecuteScalar()).ToUpper() == "TRUE";
                }
            }

            return result;
        }

        public static MemberResponse GetClaimsAddress(string ivrCode, string prefixSubscriberId, string memberSeq)
        {
            var result = new MemberResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("ClaimsAddressLookup", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = ivrCode;
                        cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = prefixSubscriberId;
                        cmd.Parameters.Add(new SqlParameter("@MemberSeq", SqlDbType.VarChar)).Value = memberSeq;
                        //output
                        var claimsAddressId = cmd.Parameters.Add("@AddressReturn", SqlDbType.Int, 4);
                        claimsAddressId.Direction = ParameterDirection.Output;
                        var claimsAddressError = cmd.Parameters.Add("@ErrorMessageLU", SqlDbType.VarChar, 500);
                        claimsAddressError.Direction = ParameterDirection.Output;
                        var claimsAddress = cmd.Parameters.Add("@Address", SqlDbType.VarChar, 200);
                        claimsAddress.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        result.IsValid = true;
                        result.ErrorMessage = string.Empty;
                        result.ClaimsAddress = DbHelper.NullString(claimsAddress.Value);
                        result.ClaimsAddressId = DbHelper.NullInt(claimsAddressId);
                        result.ClaimsAddressError = DbHelper.NullString(claimsAddressError);
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error getting Claims Address due to: " + ex.Message;
            }

            return result;
        }

        public static Dictionary<string, string> CheckBcbsmaMember(string providerId, string subscriberId, string memberSeq,
                                                    DateTime dateOfBirth, DateTime startDate)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_bcbsma_validate_member", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@member_ID", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@MemberSeq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@provider_id", SqlDbType.VarChar)).Value = providerId;
                    cmd.Parameters.Add(new SqlParameter("@dob", SqlDbType.DateTime)).Value = dateOfBirth;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = startDate;
                    //output
                    var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                    errMsg.Direction = ParameterDirection.Output;
                    var memberName = cmd.Parameters.Add("@member_name", SqlDbType.VarChar, 50);
                    memberName.Direction = ParameterDirection.Output;
                    var planMaxVisits = cmd.Parameters.Add("@plan_max_visits", SqlDbType.Int, 4);
                    planMaxVisits.Direction = ParameterDirection.Output;
                    var actualVisits = cmd.Parameters.Add("@actual_visits_for_member", SqlDbType.Int, 4);
                    actualVisits.Direction = ParameterDirection.Output;
                    var planId = cmd.Parameters.Add("@planid", SqlDbType.VarChar, 20);
                    planId.Direction = ParameterDirection.Output;
                    var ivrCode = cmd.Parameters.Add("@ivr_code", SqlDbType.VarChar, 10);
                    ivrCode.Direction = ParameterDirection.Output;
                    var prefixSubscriberId = cmd.Parameters.Add("@subscriber_Id", SqlDbType.VarChar, 20);
                    prefixSubscriberId.Direction = ParameterDirection.Output;
                    var actualStartDate = cmd.Parameters.Add("@pActual_visits_start_date", SqlDbType.DateTime, 8);
                    actualStartDate.Direction = ParameterDirection.Output;
                    var actualEndDate = cmd.Parameters.Add("@pActual_visits_end_date", SqlDbType.DateTime, 8);
                    actualEndDate.Direction = ParameterDirection.Output;
                    var visitYearType = cmd.Parameters.Add("@pVisit_Year_Type", SqlDbType.Char, 1);
                    visitYearType.Direction = ParameterDirection.Output;
                    var claimsAddressError = cmd.Parameters.Add("@claimsAddressError", SqlDbType.VarChar, 500);
                    claimsAddressError.Direction = ParameterDirection.Output;
                    var claimsAddressId = cmd.Parameters.Add("@claimsAddressId", SqlDbType.Int, 4);
                    claimsAddressId.Direction = ParameterDirection.Output;
                    var claimsAddress = cmd.Parameters.Add("@claimsAddress", SqlDbType.VarChar, 200);
                    claimsAddress.Direction = ParameterDirection.Output;


                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                    dict["member_name"] = DbHelper.NullString(memberName.Value);
                    dict["plan_max_visits"] = DbHelper.NullString(planMaxVisits.Value);
                    dict["actual_visits"] = DbHelper.NullString(actualVisits.Value);
                    dict["plan_id"] = DbHelper.NullString(planId.Value);
                    dict["ivr_code"] = DbHelper.NullString(ivrCode.Value);
                    dict["prefix_subscriber_id"] = DbHelper.NullString(prefixSubscriberId.Value);
                    dict["actual_start_date"] = DbHelper.NullString(actualStartDate.Value);
                    dict["actual_end_date"] = DbHelper.NullString(actualEndDate.Value);
                    dict["visit_year_type"] = DbHelper.NullString(visitYearType.Value);
                    dict["claimsAddressError"] = DbHelper.NullString(claimsAddressError.Value);
                    dict["claimsAddressId"] = DbHelper.NullString(claimsAddressId.Value);
                    dict["claimsAddress"] = DbHelper.NullString(claimsAddress.Value);
                    dict["dob"] = dateOfBirth.ToShortDateString();
                }
            }

            return dict;
        }

    }
}
