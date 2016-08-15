using System;
using System.Data;
using System.Data.SqlClient;
using RRS.BEL;

namespace RRS.DAL
{
    public class PendCodeAdapter
    {
        public static PendCode CheckChiroWorkInjury(ChiroAuthRequest request, Member member)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Work_Related_Denial from Plans " +
                                   "where Plan_id = @pPlan_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    var workRelatedDenial = DbHelper.NullString(cmd.ExecuteScalar()).ToUpper() == "TRUE";

                    if (request.WorkRelated && workRelatedDenial)
                    {
                        pendCode.AuthCode = "WC";
                        pendCode.AuthStatus = "D";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckPtotWorkInjury(PtotAuthRequest request, Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Work_Related_Denial from Plans " +
                                   "where Plan_id = @pPlan_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    var workRelatedDenial = DbHelper.NullString(cmd.ExecuteScalar()).ToUpper() == "TRUE";

                    if (request.WorkRelated && workRelatedDenial)
                    {
                        pendCode.AuthCode = "WC";
                        pendCode.AuthStatus = "D";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckFailToPreAuth(AuthRequest request, Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Check_Fail_To_PreAuth, Fail_To_PreAuth_Days from Plans " +
                                   "where Plan_id = @pPlan_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var failToPreAuth = DbHelper.NullString(dr[dr.GetOrdinal("Check_Fail_To_PreAuth")]).ToUpper() == "TRUE";
                            var failToPreAuthDays = DbHelper.NullInt(dr[dr.GetOrdinal("Fail_To_PreAuth_Days")]);
                            if (failToPreAuth && request.StartDate.AddDays(failToPreAuthDays) < DateTime.Today)
                            {
                                pendCode.AuthCode = "DATH";
                                pendCode.AuthStatus = "D";
                            }
                        }
                    }
                }
            }

            return pendCode;            
        }

        public static PendCode CheckProviderPend(Provider provider)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Begin_Effective, isnull(End_Effective, '1/1/2100') as End_Effective " + 
                                   "from ClaimPend " +
                                   "where Record_Type='Provider' and Record_ID=@pProvider_Id and Date_Type='Auth'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_Id", SqlDbType.VarChar)).Value = provider.Id;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var pendStartDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Begin_Effective")]));
                            var pendEndDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("End_Effective")]));
                            if (pendStartDate <= DateTime.Now && pendEndDate > DateTime.Now)
                            {
                                pendCode.AuthCode = "PDP";
                                pendCode.AuthStatus = "RCR";
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckGroupPend(Member member)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Begin_Effective, isnull(End_Effective, '1/1/2100') as End_Effective " +
                                   "from ClaimPend " +
                                   "where Record_Type='Group' and Record_ID=@pGroup_Id and Date_Type='Auth'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_Id", SqlDbType.VarChar)).Value = member.GroupId;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var pendStartDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Begin_Effective")]));
                            var pendEndDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("End_Effective")]));
                            if (pendStartDate <= DateTime.Now && pendEndDate > DateTime.Now)
                            {
                                pendCode.AuthCode = "PDN";
                                pendCode.AuthStatus = "RCR";
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckDivisionPend(Member member)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Begin_Effective, isnull(End_Effective, '1/1/2100') as End_Effective " +
                                   "from ClaimPend " +
                                   "where Record_Type='GroupDivision' and Record_ID=@pGroup_Id " +
                                   "and Record_Suffix=@pDivision_Id and Date_Type='Auth'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pGroup_Id", SqlDbType.VarChar)).Value = member.GroupId;
                    cmd.Parameters.Add(new SqlParameter("@pDivision_Id", SqlDbType.VarChar)).Value = member.DivisionId;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var pendStartDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Begin_Effective")]));
                            var pendEndDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("End_Effective")]));
                            if (pendStartDate <= DateTime.Now && pendEndDate > DateTime.Now)
                            {
                                pendCode.AuthCode = "PDD";
                                pendCode.AuthStatus = "RCR";
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckSubscriberPend(Member member)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Begin_Effective, isnull(End_Effective, '1/1/2100') as End_Effective " +
                                   "from ClaimPend " +
                                   "where Record_Type='Subscriber' and Record_ID=@pSubscriber_Id " +
                                   "and Record_Suffix='SubscriberIDOnly' and Date_Type='Auth'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var pendStartDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Begin_Effective")]));
                            var pendEndDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("End_Effective")]));
                            if (pendStartDate <= DateTime.Now && pendEndDate > DateTime.Now)
                            {
                                pendCode.AuthCode = "PDS";
                                pendCode.AuthStatus = "RCR";
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckMemberPend(Member member)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Begin_Effective, isnull(End_Effective, '1/1/2100') as End_Effective " +
                                   "from ClaimPend " +
                                   "where Record_Type='Member' and Record_ID=@pSubscriber_Id " +
                                   "and Record_Suffix=@pMember_seq and Date_Type='Auth'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var pendStartDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Begin_Effective")]));
                            var pendEndDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("End_Effective")]));
                            if (pendStartDate <= DateTime.Now && pendEndDate > DateTime.Now)
                            {
                                pendCode.AuthCode = "PDS";
                                pendCode.AuthStatus = "RCR";
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckPatientAge(Member member)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select Pre_Auth_Min_Age from Plans " +
                          "where Plan_id=@pPlan_id " + Environment.NewLine +
                          "select Birth_Date from Members " +
                          "where Subscriber_ID=@pSubscriber_Id and Member_Seq=@pMember_seq";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_id", SqlDbType.VarChar)).Value = member.PlanId;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        var minAge = 0;
                        while (dr.Read())
                        {
                            minAge = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Pre_Auth_Min_Age")]));
                        }

                        dr.NextResult();

                        var birthDate = new DateTime(1900, 1, 1);
                        while (dr.Read())
                        {
                            birthDate = DateTime.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Birth_Date")]));
                        }

                        if (DateTime.Now.Subtract(birthDate).TotalDays < minAge * 365)
                        {
                            pendCode.AuthCode = "UA";
                            pendCode.AuthStatus = "RCR";
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckPendingAuth(Member member, Provider provider)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as auth_count from Auth " +
                                   "where Subscriber_ID=@pSubscriber_Id and Member_Seq=@pMember_seq " +
                                   "and Provider_ID=@pProvider_ID and ProviderLocation_Seq=@pProviderLocation_Seq " +
                                   "and AuthType_ID=@pAuthType_ID and [Status]='RCR'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_ID", SqlDbType.VarChar)).Value = provider.Id;
                    cmd.Parameters.Add(new SqlParameter("@pProviderLocation_Seq", SqlDbType.VarChar)).Value = provider.LocationSeq;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    conn.Open();

                    var authCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (authCount > 0)
                    {
                        pendCode.AuthCode = "PA";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;            
        }

        public static PendCode CheckRecentAuth(Member member, int daysBack)
        {
            var pendCode = new PendCode {AuthCode = string.Empty, AuthStatus = string.Empty};

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as auth_count from Auth " +
                                   "where Subscriber_ID=@pSubscriber_Id and Member_Seq=@pMember_seq " +
                                   "and AuthType_ID=@pAuthType_ID and Auth_Code<>'CRA' " + 
                                   "and datediff(day, Entry_Date, getdate())<=@iDaysBack and [Status]<>'V'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@iDaysBack", SqlDbType.Int)).Value = daysBack;
                    conn.Open();

                    var authCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (authCount > 0)
                    {
                        pendCode.AuthCode = "CA";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckVisitsToDate(AuthRequest request, Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Init_Auth_Prev_Visit_Limit, Prev_Visit_Limit from Plans " +
                                   "where Plan_id = @pPlan_id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_id", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var newAuthVisitLimit =
                                int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Init_Auth_Prev_Visit_Limit")]));
                            var regularVisitLimit = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Prev_Visit_Limit")]));

                            var requestType = request.RequestType;
                            if (member.UnmanagedVisits > 0)
                                requestType = "2";

                            // 1 = new auth; 2 = continuation of care
                            if (requestType.Equals("1"))
                            {
                                if (request.ActualVisits > newAuthVisitLimit)
                                {
                                    pendCode.AuthCode = "PV";
                                    pendCode.AuthStatus = "RCR";
                                }
                            }
                            else
                            {
                                if (request.ActualVisits > regularVisitLimit)
                                {
                                    pendCode.AuthCode = "VC";
                                    pendCode.AuthStatus = "RCR";
                                }
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        private static DateTime GetLimitDateFromMonth(AuthRequest request, Member member, int monthsBack)
        {
            var yearStartDate = new DateTime(request.StartDate.Year, 1, 1);
            var limitDate = request.StartDate.AddMonths(monthsBack*-1);

            if (member.LimitToYear && limitDate < yearStartDate)
                limitDate = yearStartDate;

            return limitDate;
        }

        private static DateTime GetLimitDateFromDay(AuthRequest request, Member member, int daysBack)
        {
            var yearStartDate = new DateTime(request.StartDate.Year, 1, 1);
            var limitDate = request.StartDate.AddDays(daysBack * -1);

            if (member.LimitToYear && limitDate < yearStartDate)
                limitDate = yearStartDate;

            return limitDate;
        }

        public static PendCode CheckMemberDenied(AuthRequest request, Member member, int monthsBack)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as denied_count " +
                                   "from Auth a inner join AuthCode ac on a.Auth_Code=ac.Auth_Code " +
                                   "where a.Subscriber_ID=@pSubscriber_Id " +
                                   "and a.Member_Seq=@pMember_seq " + 
                                   "and a.AuthType_ID=@pAuthType_ID " + 
                                   "and a.Status='D' and a.Auth_code<>'CRA' " +
                                   "and a.Update_date>=@iLimit_Date " +
                                   "and ac.Decision_Type='MN' and ac.Reduction_Ind=0";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@iLimit_Date", SqlDbType.DateTime)).Value = GetLimitDateFromMonth(request, member, monthsBack);
                    conn.Open();

                    var deniedCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (deniedCount > 0)
                    {
                        pendCode.AuthCode = "DF";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckPreviousAuths(AuthRequest request, Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };
            var multiAuthDayLimit = 0;
            var multiAuthNumLimit = 0;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select Multi_Auth_Request_Day_Limit, Multi_Auth_Request_Num_Limit " +
                          "from CompanyData";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            multiAuthDayLimit =
                                int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Multi_Auth_Request_Day_Limit")]));
                            multiAuthNumLimit =
                                int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Multi_Auth_Request_Num_Limit")]));
                        }
                    }
                }

                if (multiAuthDayLimit != 0)
                {
                    sql = "select count(*) as multi_auth_count from Auth " +
                          "where Subscriber_ID=@pSubscriber_Id and Member_Seq=@pMember_seq " +
                          "and AuthType_ID=@pAuthType_ID and Auth_Code<>'CRA' " + 
                          "and Approved_Date>=@iLimit_Date and [Status]<>'V'";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                        cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                        cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                        cmd.Parameters.Add(new SqlParameter("@iLimit_Date", SqlDbType.DateTime)).Value = GetLimitDateFromDay(request, member, multiAuthDayLimit);
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        member.NumberOfPreviousAuths = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                        if (member.NumberOfPreviousAuths > multiAuthNumLimit)
                        {
                            pendCode.AuthCode = "MA";
                            pendCode.AuthStatus = "RCR";
                        }
                        else
                        {
                            member.NumberOfPreviousAuths = 0;
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckFutureAuths(AuthRequest request, Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as future_auths_count from Auth " +
                                   "where Subscriber_ID=@pSubscriber_Id " +
                                   "and Member_Seq=@pMember_seq " +
                                   "and AuthType_ID=@pAuthType_ID " + 
                                   "and Requested_From>=@pStart_Date_Requested " +
                                   "and [Status]<>'V'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    conn.Open();

                    var futureAuthCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (futureAuthCount > 0)
                    {
                        pendCode.AuthCode = "FA";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckDecisionType(AuthRequest request, Member member, int daysBack)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as decision_type_count " +
                                   "from Auth a inner join AuthCode ac on a.Auth_Code=ac.Auth_Code " +
                                   "where a.Subscriber_ID=@pSubscriber_Id " +
                                   "and a.Member_Seq=@pMember_seq " +
                                   "and a.AuthType_ID=@pAuthType_ID " + 
                                   "and a.Approved_Date>@iLimit_Date " +
                                   "and ac.Decision_Type in ('AD','MN') " +
                                   "and a.Auth_Code<>'CRA' and a.[Status]<>'V'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pAuthType_ID", SqlDbType.VarChar)).Value = member.AuthTypeId;
                    cmd.Parameters.Add(new SqlParameter("@iLimit_Date", SqlDbType.DateTime)).Value = GetLimitDateFromDay(request, member, daysBack);
                    conn.Open();

                    var decisionTypeCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (decisionTypeCount > 0)
                    {
                        pendCode.AuthCode = "PP";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckOtherProviders(AuthRequest request, Member member, Provider provider)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as providers_count from Auth " +
                                   "where Subscriber_ID=@pSubscriber_Id " +
                                   "and Member_Seq=@pMember_seq " +
                                   "and Year(Coalesce(Approved_From, Requested_From))=Year(@pStart_Date_Requested) " +
                                   "and Provider_ID<>@pProvider_id " +
                                   "and [Status]<>'V'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pSubscriber_Id", SqlDbType.VarChar)).Value = member.SubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = member.MemberSeq;
                    cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_id", SqlDbType.VarChar)).Value = provider.Id;
                    conn.Open();

                    var providersCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (providersCount > 0)
                    {
                        pendCode.AuthCode = "OP";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckPcpReferral(Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select PCP_Referral_Required from Plans " +
                                   "where plan_id=@pPlan_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    var pcpRefRequired = DbHelper.NullString(cmd.ExecuteScalar()).ToUpper() == "TRUE";
                    if (pcpRefRequired)
                    {
                        pendCode.AuthCode = "PCPR";
                        pendCode.AuthStatus = "RCR";
                    }
                }                
            }

            return pendCode;
        }

        public static PendCode CheckMedicareAdvantage(AuthRequest request, Member member)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select Medicare_Advantage_Special_Handling from Plans where plan_id=@pPlan_ID";

                bool isMedicareAdvantage;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pPlan_ID", SqlDbType.VarChar)).Value = member.PlanId;
                    conn.Open();

                    isMedicareAdvantage = DbHelper.NullString(cmd.ExecuteScalar()).ToUpper() == "TRUE";
                }

                if (isMedicareAdvantage)
                {
                    sql = "select count(*) as primary_count from DiagnosticCode_MAM " +
                          "where Diagnostic_Code=@pPrimary_code and Type=1 and Valid=1 " + Environment.NewLine +
                          "select count(*) as secondary_count from DiagnosticCode_MAM " +
                          "where Diagnostic_Code=@pSecondary_code and Type=2 and Valid=1";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@pPrimary_code", SqlDbType.VarChar)).Value = request.PrimaryDiagnosisCode;
                        cmd.Parameters.Add(new SqlParameter("@pSecondary_code", SqlDbType.VarChar)).Value = request.SecondaryDiagnosisCode;
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        using (var dr = cmd.ExecuteReader())
                        {
                            var primaryCount = 0;
                            while (dr.Read())
                            {
                                primaryCount = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("primary_count")]));
                            }

                            dr.NextResult();

                            var secondaryCount = 0;
                            while (dr.Read())
                            {
                                secondaryCount = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("secondary_count")]));
                            }

                            if (primaryCount == 0 || secondaryCount == 0)
                            {
                                pendCode.AuthCode = "PMAM";
                                pendCode.AuthStatus = "RCR";
                            }
                        }
                    }
                }
            }

            return pendCode;
        }

        public static PendCode CheckRetroReview(AuthRequest request)
        {
            var pendCode = new PendCode { AuthCode = string.Empty, AuthStatus = string.Empty };

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select Auth_Approved_From_Days_Back from CompanyData";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    var backDays = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    var dayDiff = DateTime.Now - request.StartDate;
                    if (dayDiff.Days > backDays)
                    {
                        pendCode.AuthCode = "PRR";
                        pendCode.AuthStatus = "RCR";
                    }
                }
            }

            return pendCode;
        }
    }
}
