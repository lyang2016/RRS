using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RRS.BEL;

namespace RRS.DAL
{
    public class ProviderAdapter
    {
        public static bool UpdateFax(string providerId, string locationSeq, string fax)
        {
            var result = true;

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("rrs_bcbsma_update_provider_fax", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;
                        cmd.Parameters.Add(new SqlParameter("@Location_Seq", SqlDbType.VarChar)).Value = locationSeq;
                        cmd.Parameters.Add(new SqlParameter("@Fax", SqlDbType.VarChar)).Value = fax;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }                
            }
            catch(Exception)
            {
                result = false;
            }

            return result;
        }

        public static Dictionary<string, string> CheckProviderFromSso(string clientPrefix, string id, DateTime requestedStartDate)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select top 1 pa.IVR_Code " + Environment.NewLine +
                          "from ProviderAddress pa inner join ProviderNetwork pn " + Environment.NewLine +
                          "on pa.Provider_ID=pn.Provider_ID and pa.Location_Seq=pn.Location_Seq " + Environment.NewLine +
                          "where pa.Provider_ID=@pProvider_Id AND pn.Eff_Date IS NOT NULL " + Environment.NewLine +
                          "AND (pn.Term_Date IS NULL OR pn.Term_Date>=@pRequestedStartDate)";

                string ivrCode;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pProvider_Id", SqlDbType.VarChar)).Value = clientPrefix + id;
                    cmd.Parameters.Add(new SqlParameter("@pRequestedStartDate", SqlDbType.DateTime)).Value = requestedStartDate;
                    conn.Open();

                    ivrCode = DbHelper.NullString(cmd.ExecuteScalar());
                }

                if (ivrCode.Equals(string.Empty))
                {
                    dict["err_num"] = "Y";
                    dict["errormessage"] = "Provider not found.";
                }
                else
                {
                    dict["err_num"] = "N";
                    dict["errormessage"] = string.Empty;
                    dict["ivr_code"] = ivrCode;
                    dict["provider_id"] = clientPrefix + id;
                    dict["authtype_id"] = "";

                    /* WI #12101
                    sql = "select top 1 na.AuthType_ID from ProviderNetwork pn " + Environment.NewLine +
                          "inner join NetworkAuthType na on pn.Network_ID=na.Network_ID " + Environment.NewLine +
                          "where pn.Provider_ID=@pProvider_ID " + Environment.NewLine +
                          "order by na.AuthType_ID asc";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@pProvider_ID", SqlDbType.VarChar)).Value = clientPrefix + id;
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        dict["authtype_id"] = DbHelper.NullString(cmd.ExecuteScalar());
                        if (!dict["authtype_id"].Equals("DC"))
                        {
                            dict["authtype_id"] = "PTOT";
                        }
                    }
                     */
                }
            }

            return dict;
        }

        public static Dictionary<string, string> CheckProvider(string code, bool isIvr, string userId="", string password="")
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                if (isIvr)
                {
                    using (var cmd = new SqlCommand("rrs_Validate_Provider_IVR", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = code;
                        //output
                        var errNum = cmd.Parameters.Add("@err_num", SqlDbType.Int, 4);
                        errNum.Direction = ParameterDirection.Output;
                        var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                        errMsg.Direction = ParameterDirection.Output;
                        var providerId = cmd.Parameters.Add("@Provider_ID", SqlDbType.VarChar, 20);
                        providerId.Direction = ParameterDirection.Output;
                        var locationSeq = cmd.Parameters.Add("@Location_Seq", SqlDbType.VarChar, 2);
                        locationSeq.Direction = ParameterDirection.Output;
                        var zip = cmd.Parameters.Add("@ZipCode5", SqlDbType.VarChar, 5);
                        zip.Direction = ParameterDirection.Output;
                        var tin = cmd.Parameters.Add("@TINLast4", SqlDbType.VarChar, 4);
                        tin.Direction = ParameterDirection.Output;
                        var fax = cmd.Parameters.Add("@Fax_Number", SqlDbType.VarChar, 10);
                        fax.Direction = ParameterDirection.Output;
                        var isBcbsma = cmd.Parameters.Add("@isBCBSMA", SqlDbType.Bit, 1);
                        isBcbsma.Direction = ParameterDirection.Output;
                        var ivr1 = cmd.Parameters.Add("@ivr_right_1", SqlDbType.Bit, 1);
                        ivr1.Direction = ParameterDirection.Output;
                        var ivr2 = cmd.Parameters.Add("@ivr_right_2", SqlDbType.Bit, 1);
                        ivr2.Direction = ParameterDirection.Output;
                        var ivr3 = cmd.Parameters.Add("@ivr_right_3", SqlDbType.Bit, 1);
                        ivr3.Direction = ParameterDirection.Output;
                        var ivr4 = cmd.Parameters.Add("@ivr_right_4", SqlDbType.Bit, 1);
                        ivr4.Direction = ParameterDirection.Output;
                        var ivr5 = cmd.Parameters.Add("@ivr_right_5", SqlDbType.Bit, 1);
                        ivr5.Direction = ParameterDirection.Output;
                        var ivr6 = cmd.Parameters.Add("@ivr_right_6", SqlDbType.Bit, 1);
                        ivr6.Direction = ParameterDirection.Output;
                        var ivr7 = cmd.Parameters.Add("@ivr_right_7", SqlDbType.Bit, 1);
                        ivr7.Direction = ParameterDirection.Output;
                        var playIntro = cmd.Parameters.Add("@PlayBCBSMAIntro", SqlDbType.Bit, 1);
                        playIntro.Direction = ParameterDirection.Output;
                        var playExclude = cmd.Parameters.Add("@PlayExcludeAlphaPrefix", SqlDbType.Bit, 1);
                        playExclude.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        dict["err_num"] = DbHelper.NullString(errNum.Value);
                        dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                        dict["Provider_ID"] = DbHelper.NullString(providerId.Value);
                        dict["Location_Seq"] = DbHelper.NullString(locationSeq.Value);
                        dict["Zip"] = DbHelper.NullString(zip.Value);
                        dict["TIN"] = DbHelper.NullString(tin.Value);
                        dict["Fax_Number"] = DbHelper.NullString(fax.Value);
                        dict["isBCBSMA"] = DbHelper.NullString(isBcbsma.Value);
                        dict["ivr_right_1"] = DbHelper.NullString(ivr1.Value);
                        dict["ivr_right_2"] = DbHelper.NullString(ivr2.Value);
                        dict["ivr_right_3"] = DbHelper.NullString(ivr3.Value);
                        dict["ivr_right_4"] = DbHelper.NullString(ivr4.Value);
                        dict["ivr_right_5"] = DbHelper.NullString(ivr5.Value);
                        dict["ivr_right_6"] = DbHelper.NullString(ivr6.Value);
                        dict["ivr_right_7"] = DbHelper.NullString(ivr7.Value);
                        dict["PlayBCBSMAIntro"] = DbHelper.NullString(playIntro.Value);
                        dict["PlayExcludeAlphaPrefix"] = DbHelper.NullString(playExclude.Value);
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand("rrs_Validate_Provider", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.VarChar)).Value = userId;
                        cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar)).Value = password;
                        cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = code;
                        //output
                        var errNum = cmd.Parameters.Add("@err", SqlDbType.Char, 1);
                        errNum.Direction = ParameterDirection.Output;
                        var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                        errMsg.Direction = ParameterDirection.Output;
                        var providerName = cmd.Parameters.Add("@provider_name", SqlDbType.VarChar, 60);
                        providerName.Direction = ParameterDirection.Output;
                        var address = cmd.Parameters.Add("@address", SqlDbType.VarChar, 90);
                        address.Direction = ParameterDirection.Output;
                        var city = cmd.Parameters.Add("@city", SqlDbType.VarChar, 50);
                        city.Direction = ParameterDirection.Output;
                        var state = cmd.Parameters.Add("@state", SqlDbType.Char, 2);
                        state.Direction = ParameterDirection.Output;
                        var zip = cmd.Parameters.Add("@zip", SqlDbType.VarChar, 10);
                        zip.Direction = ParameterDirection.Output;
                        var fax = cmd.Parameters.Add("@fax_number", SqlDbType.VarChar, 10);
                        fax.Direction = ParameterDirection.Output;
                        var isGhc = cmd.Parameters.Add("@isghc", SqlDbType.Char, 1);
                        isGhc.Direction = ParameterDirection.Output;
                        var isJdhp = cmd.Parameters.Add("@isjdhp", SqlDbType.Char, 1);
                        isJdhp.Direction = ParameterDirection.Output;
                        var ivr1 = cmd.Parameters.Add("@ivr_right_1", SqlDbType.Char, 8);
                        ivr1.Direction = ParameterDirection.Output;
                        var ivr2 = cmd.Parameters.Add("@ivr_right_2", SqlDbType.Char, 8);
                        ivr2.Direction = ParameterDirection.Output;
                        var ivr3 = cmd.Parameters.Add("@ivr_right_3", SqlDbType.Char, 8);
                        ivr3.Direction = ParameterDirection.Output;
                        var ivr4 = cmd.Parameters.Add("@ivr_right_4", SqlDbType.Char, 8);
                        ivr4.Direction = ParameterDirection.Output;
                        var ivr5 = cmd.Parameters.Add("@ivr_right_5", SqlDbType.Char, 8);
                        ivr5.Direction = ParameterDirection.Output;
                        var ivr6 = cmd.Parameters.Add("@ivr_right_6", SqlDbType.Char, 8);
                        ivr6.Direction = ParameterDirection.Output;
                        var ivr7 = cmd.Parameters.Add("@ivr_right_7", SqlDbType.Char, 8);
                        ivr7.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        dict["err_num"] = DbHelper.NullString(errNum.Value);
                        dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                        dict["provider_name"] = DbHelper.NullString(providerName.Value);
                        dict["address"] = DbHelper.NullString(address.Value);
                        dict["city"] = DbHelper.NullString(city.Value);
                        dict["state"] = DbHelper.NullString(state.Value);
                        dict["Zip"] = DbHelper.NullString(zip.Value);
                        dict["Fax_Number"] = DbHelper.NullString(fax.Value);
                        dict["isghc"] = DbHelper.NullString(isGhc.Value);
                        dict["isjdhp"] = DbHelper.NullString(isJdhp.Value);
                        dict["ivr_right_1"] = DbHelper.NullString(ivr1.Value);
                        dict["ivr_right_2"] = DbHelper.NullString(ivr2.Value);
                        dict["ivr_right_3"] = DbHelper.NullString(ivr3.Value);
                        dict["ivr_right_4"] = DbHelper.NullString(ivr4.Value);
                        dict["ivr_right_5"] = DbHelper.NullString(ivr5.Value);
                        dict["ivr_right_6"] = DbHelper.NullString(ivr6.Value);
                        dict["ivr_right_7"] = DbHelper.NullString(ivr7.Value);
                    }
                }
            }

            return dict;
        }

        public static Dictionary<string, string> AngelCheckProvider(string accessCode)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_Validate_Provider_Angel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = accessCode;
                    //output
                    var errNum = cmd.Parameters.Add("@err_num", SqlDbType.Int, 4);
                    errNum.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@errormessage", SqlDbType.VarChar, 90);
                    errMsg.Direction = ParameterDirection.Output;
                    var providerId = cmd.Parameters.Add("@Provider_ID", SqlDbType.VarChar, 20);
                    providerId.Direction = ParameterDirection.Output;
                    var locationSeq = cmd.Parameters.Add("@Location_Seq", SqlDbType.VarChar, 2);
                    locationSeq.Direction = ParameterDirection.Output;
                    var zip = cmd.Parameters.Add("@ZipCode5", SqlDbType.VarChar, 5);
                    zip.Direction = ParameterDirection.Output;
                    var tin = cmd.Parameters.Add("@TINLast4", SqlDbType.VarChar, 4);
                    tin.Direction = ParameterDirection.Output;
                    var npi = cmd.Parameters.Add("@NPILast4", SqlDbType.VarChar, 4);
                    npi.Direction = ParameterDirection.Output;
                    var fax = cmd.Parameters.Add("@Fax_Number", SqlDbType.VarChar, 10);
                    fax.Direction = ParameterDirection.Output;
                    var isBcbsma = cmd.Parameters.Add("@isBCBSMA", SqlDbType.Bit, 1);
                    isBcbsma.Direction = ParameterDirection.Output;
                    var isHighmark = cmd.Parameters.Add("@isHighmark", SqlDbType.Bit, 1);
                    isHighmark.Direction = ParameterDirection.Output;
                    var ivr1 = cmd.Parameters.Add("@ivr_right_1", SqlDbType.Bit, 1);
                    ivr1.Direction = ParameterDirection.Output;
                    var ivr2 = cmd.Parameters.Add("@ivr_right_2", SqlDbType.Bit, 1);
                    ivr2.Direction = ParameterDirection.Output;
                    var ivr3 = cmd.Parameters.Add("@ivr_right_3", SqlDbType.Bit, 1);
                    ivr3.Direction = ParameterDirection.Output;
                    var ivr4 = cmd.Parameters.Add("@ivr_right_4", SqlDbType.Bit, 1);
                    ivr4.Direction = ParameterDirection.Output;
                    var ivr5 = cmd.Parameters.Add("@ivr_right_5", SqlDbType.Bit, 1);
                    ivr5.Direction = ParameterDirection.Output;
                    var ivr6 = cmd.Parameters.Add("@ivr_right_6", SqlDbType.Bit, 1);
                    ivr6.Direction = ParameterDirection.Output;
                    var ivr7 = cmd.Parameters.Add("@ivr_right_7", SqlDbType.Bit, 1);
                    ivr7.Direction = ParameterDirection.Output;
                    var playIntro = cmd.Parameters.Add("@PlayBCBSMAIntro", SqlDbType.Bit, 1);
                    playIntro.Direction = ParameterDirection.Output;
                    var playExclude = cmd.Parameters.Add("@PlayExcludeAlphaPrefix", SqlDbType.Bit, 1);
                    playExclude.Direction = ParameterDirection.Output;
                    var isDean = cmd.Parameters.Add("@isDean", SqlDbType.Bit, 1);
                    isDean.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["err_num"] = DbHelper.NullString(errNum.Value);
                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                    dict["Provider_ID"] = DbHelper.NullString(providerId.Value);
                    dict["Location_Seq"] = DbHelper.NullString(locationSeq.Value);
                    dict["Zip"] = DbHelper.NullString(zip.Value);
                    dict["TIN"] = DbHelper.NullString(tin.Value);
                    dict["NPI"] = DbHelper.NullString(npi.Value);
                    dict["Fax_Number"] = DbHelper.NullString(fax.Value);
                    dict["isBCBSMA"] = DbHelper.NullString(isBcbsma.Value);
                    dict["isHighmark"] = DbHelper.NullString(isHighmark.Value);
                    dict["ivr_right_1"] = DbHelper.NullString(ivr1.Value);
                    dict["ivr_right_2"] = DbHelper.NullString(ivr2.Value);
                    dict["ivr_right_3"] = DbHelper.NullString(ivr3.Value);
                    dict["ivr_right_4"] = DbHelper.NullString(ivr4.Value);
                    dict["ivr_right_5"] = DbHelper.NullString(ivr5.Value);
                    dict["ivr_right_6"] = DbHelper.NullString(ivr6.Value);
                    dict["ivr_right_7"] = DbHelper.NullString(ivr7.Value);
                    dict["PlayBCBSMAIntro"] = DbHelper.NullString(playIntro.Value);
                    dict["PlayExcludeAlphaPrefix"] = DbHelper.NullString(playExclude.Value);
                    dict["isDean"] = DbHelper.NullString(isDean.Value);
                }
            }
            
            return dict;
        }

        public static Provider GetProviderInfo(AuthRequest request, string authType)
        {
            var provider = new Provider();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "select p.provider_id, pa.location_seq, p.first_name, p.last_name " +
                      "from ProviderAddress pa inner join Provider p on pa.provider_id=p.provider_id " +
                      "where pa.IVR_code = @pIvr_code";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pIvr_code", SqlDbType.VarChar)).Value = request.IvrCode;
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            provider.Id = DbHelper.NullString(dr[dr.GetOrdinal("provider_id")]);
                            provider.LocationSeq = DbHelper.NullString(dr[dr.GetOrdinal("location_seq")]);
                            provider.FirstName = DbHelper.NullString(dr[dr.GetOrdinal("first_name")]);
                            provider.LastName = DbHelper.NullString(dr[dr.GetOrdinal("last_name")]);
                        }
                    }
                }

                sql = "SELECT count(*) from ProviderAuthWaiver " +
                      "WHERE [Start_Date]<=@pStart_Date_Requested " +
                      "AND ([End_Date] Is Null Or [End_Date]>= @pStart_Date_Requested) " +
                      "AND Provider_ID=@iProvider_id";

                if (!provider.Id.Equals(string.Empty))
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@pStart_Date_Requested", SqlDbType.DateTime)).Value = request.StartDate;
                        cmd.Parameters.Add(new SqlParameter("@iProvider_id", SqlDbType.VarChar)).Value = provider.Id;
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        provider.AuthWaiver = int.Parse(DbHelper.NullString(cmd.ExecuteScalar())) > 0;
                    }
                }

                sql = "SELECT GN.NetworkID AS Matching_Network_ID " + Environment.NewLine +
                      "FROM SubEnrollment SE (NOLOCK) " + Environment.NewLine +
                      "INNER JOIN GroupNetwork GN (NOLOCK) " + Environment.NewLine +
                      "ON SE.Group_ID = GN.group_id AND SE.Division_ID = GN.division_id " + Environment.NewLine +
                      "AND SE.Group_Coverage_Start = GN.coverage_start " + Environment.NewLine +
                      "INNER JOIN Networks N (NOLOCK) ON GN.NetworkID = N.Network_ID " + Environment.NewLine +
                      "INNER JOIN ProviderNetwork PN (NOLOCK) ON PN.Network_ID = GN.NetworkID " + Environment.NewLine +
                      "INNER Join ProviderAddress PA (NOLOCK) ON PN.Provider_ID = PA.Provider_ID AND PN.Location_Seq = PN.Location_Seq " +
                      Environment.NewLine +
                      "INNER JOIN Plans P (NOLOCK) ON SE.Plan_ID = P.Plan_ID " + Environment.NewLine +
                      "WHERE SE.Subscriber_ID = @subscriber_id " + Environment.NewLine +
                      "AND SE.Void = 0 AND SE.Start_Date <= @requestedStartDate " + Environment.NewLine +
                      "AND (SE.End_Date IS NULL OR SE.End_Date >= @requestedStartDate) " + Environment.NewLine +
                      "AND PA.ivr_code = @ivr_code " + Environment.NewLine +
                      "AND PN.Eff_Date <= @requestedStartDate " + Environment.NewLine +
                      "AND (PN.Term_Date > @requestedStartDate OR PN.Term_Date IS NULL) " + Environment.NewLine +
                      "AND P.AuthType_ID = @Real_AuthType_ID";

                if (!provider.Id.Equals(string.Empty))
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = request.SubscriberId;
                        cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = request.StartDate;
                        cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = request.IvrCode;
                        cmd.Parameters.Add(new SqlParameter("@Real_AuthType_ID", SqlDbType.VarChar)).Value = authType;

                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        provider.NetworkId = DbHelper.NullString(cmd.ExecuteScalar());
                    }
                }
            }

            return provider;
        }

        public static ProviderResponse ViewProviderInfo(string ivrCode)
        {
            var result = new ProviderResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    const string sql = "select p.First_Name, p.Last_Name, pa.Address_1, pa.Address_2, pa.City, pa.State, " +
                                       "pa.Zip_Code, pa.IVR_Code, p.Provider_ID, p.Suffix " +
                                       "from Provider p left outer join ProviderAddress pa on p.Provider_ID=pa.Provider_ID " +
                                       "where pa.IVR_Code=@IVR_Code";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@IVR_Code", SqlDbType.VarChar)).Value = ivrCode;

                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                result.IsValid = true;
                                result.ErrorMessage = string.Empty;

                                result.FirstName = DbHelper.NullString(dr[dr.GetOrdinal("First_Name")]);
                                result.LastName = DbHelper.NullString(dr[dr.GetOrdinal("Last_Name")]);
                                result.Address1 = DbHelper.NullString(dr[dr.GetOrdinal("Address_1")]);
                                result.Address2 = DbHelper.NullString(dr[dr.GetOrdinal("Address_2")]);
                                result.City = DbHelper.NullString(dr[dr.GetOrdinal("City")]);
                                result.State = DbHelper.NullString(dr[dr.GetOrdinal("State")]);
                                result.ZipCode = DbHelper.NullString(dr[dr.GetOrdinal("Zip_Code")]);
                                result.IvrCode = DbHelper.NullString(dr[dr.GetOrdinal("IVR_Code")]);
                                result.ProviderId = DbHelper.NullString(dr[dr.GetOrdinal("Provider_ID")]);
                                result.Suffix = DbHelper.NullString(dr[dr.GetOrdinal("Suffix")]);
                            }
                            else
                            {
                                result.IsValid = false;
                                result.ErrorMessage = "No record exists for provider IVR code = " + ivrCode;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error viewing provider info due to: " + ex.Message;
            }

            return result;
        }

        public static ProviderResponse GetProviderFax(string providerId)
        {
            var result = new ProviderResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    const string sql = "select Fax, Provider_ID, Location_Seq from ProviderAddress " +
                                       "where Provider_ID=@Provider_ID";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;

                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                result.IsValid = true;
                                result.ErrorMessage = string.Empty;

                                result.Fax = DbHelper.NullString(dr[dr.GetOrdinal("Fax")]);
                                result.ProviderId = DbHelper.NullString(dr[dr.GetOrdinal("Provider_ID")]);
                                result.LocationSeq = DbHelper.NullString(dr[dr.GetOrdinal("Location_Seq")]);
                            }
                            else
                            {
                                result.IsValid = false;
                                result.ErrorMessage = "No record exists for provider ID = " + providerId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error getting provider fax due to: " + ex.Message;
            }

            return result;
        }

        public static DataSet GetAddressList(string providerId)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "SELECT DISTINCT pa.Address_1, pa.IVR_Code " + Environment.NewLine +
                          "FROM ProviderNetwork pn INNER JOIN ProviderAddress pa ON pn.Provider_ID=pa.Provider_ID " +
                          "AND pn.Location_Seq=pa.Location_Seq " + Environment.NewLine +
                          "WHERE pn.Provider_ID=@Provider_ID AND pn.Eff_Date IS NOT NULL " +
                          "AND (pn.Term_Date IS NULL OR pn.Term_Date>=GETDATE())";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "ProviderAddresses");
                    }
                }
            }

            return ds;
        }

        public static DataSet GetAddressListFromSso(string providerId, DateTime requestedStartDate)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "SELECT DISTINCT pa.Address_1, pa.IVR_Code " + Environment.NewLine +
                          "FROM ProviderNetwork pn INNER JOIN ProviderAddress pa ON pn.Provider_ID=pa.Provider_ID " +
                          "AND pn.Location_Seq=pa.Location_Seq " + Environment.NewLine +
                          "WHERE pn.Provider_ID=@Provider_ID AND pn.Eff_Date IS NOT NULL " +
                          "AND (pn.Term_Date IS NULL OR pn.Term_Date>=@pRequestedStartDate)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;
                    cmd.Parameters.Add(new SqlParameter("@pRequestedStartDate", SqlDbType.DateTime)).Value = requestedStartDate;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "ProviderAddresses");
                    }
                }
            }

            return ds;
        }

        public static DataSet GetAllMatchedPtotAuthType(string ivrCode, string subscriberId, string memberSeq, DateTime requestedStartDate)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_GetMatchPTOTAuthType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = ivrCode;
                    cmd.Parameters.Add(new SqlParameter("@subscriber_id", SqlDbType.VarChar)).Value = subscriberId;
                    cmd.Parameters.Add(new SqlParameter("@member_seq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = requestedStartDate;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "PtotAuthType");
                    }
                }
            }

            return ds;            
        }

        public static DataSet GetAllMatchedPtotAuthTypeFromSso(string ivrCode, string clientPrefix, string subscriberId, string blindKey, DateTime requestedStartDate)
        {
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

                return GetAllMatchedPtotAuthType(ivrCode, subscriberId, memberSeq, requestedStartDate);
            }
        }

        public static DataSet GetAllServiceType(string ivrCode, string prefixSubscriberId, string memberSeq, DateTime requestedStartDate)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_GetAllServiceType", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ivr_code", SqlDbType.VarChar)).Value = ivrCode;
                    cmd.Parameters.Add(new SqlParameter("@prefix_subscriber_id", SqlDbType.VarChar)).Value = prefixSubscriberId;
                    cmd.Parameters.Add(new SqlParameter("@member_seq", SqlDbType.VarChar)).Value = memberSeq;
                    cmd.Parameters.Add(new SqlParameter("@requestedStartDate", SqlDbType.DateTime)).Value = requestedStartDate;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "ServiceType");
                    }
                }
            }

            return ds;
        }

        public static DataSet GetBcbsmaProviderList()
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "SELECT First_Name + ' ' + Last_Name AS Name, BCBSMA_Provider_ID, BCBSMA_Group_Indicator, " + Environment.NewLine +
                          "BCBSMA_Provider_Type_ID, Last_Name, First_Name, Provider_ID, Suffix " + Environment.NewLine +
                          "FROM dbo.Provider " + Environment.NewLine +
                          "WHERE BCBSMA_Provider_ID IS NOT NULL";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "BcbsmaProviders");
                    }
                }
            }

            return ds;
        }

        public static DataSet GetBcbsmaProviderGroup(string providerId)
        {
            var ds = new DataSet();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "SELECT Provider_ID, ProviderGroup_ID, dt_start, dt_end " + Environment.NewLine +
                          "FROM dbo.ProviderGroups " + Environment.NewLine +
                          "WHERE Provider_ID = @Provider_ID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "BcbsmaProviderGroups");
                    }
                }
            }

            return ds;
        }

        public static Dictionary<string, string> ValidateBcbsmaSso(string bcbsmaProviderId)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_bcbsma_SSO_validation_wcf", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@BCBSMA_Provider_ID", SqlDbType.VarChar)).Value = bcbsmaProviderId;
                    //output
                    var providerId = cmd.Parameters.Add("@provider_id", SqlDbType.VarChar, 20);
                    providerId.Direction = ParameterDirection.Output;
                    var groupIndicator = cmd.Parameters.Add("@BCBSMA_Group_Indicator", SqlDbType.TinyInt, 1);
                    groupIndicator.Direction = ParameterDirection.Output;
                    var providerType = cmd.Parameters.Add("@BCBSMA_Provider_Type", SqlDbType.VarChar, 1);
                    providerType.Direction = ParameterDirection.Output;
                    var ivrCode = cmd.Parameters.Add("@ivr_code", SqlDbType.VarChar, 10);
                    ivrCode.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@error_message", SqlDbType.VarChar, 400);
                    errMsg.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    dict["Provider_ID"] = DbHelper.NullString(providerId.Value);
                    dict["Group_Indicator"] = DbHelper.NullString(groupIndicator.Value);
                    dict["Provider_Type"] = DbHelper.NullString(providerType.Value);
                    dict["ivr_code"] = DbHelper.NullString(ivrCode.Value);
                    dict["errormessage"] = DbHelper.NullString(errMsg.Value);
                }
            }

            return dict;
        }

        public static ProviderResponse ViewBcbsmaProviderInfo(string providerId)
        {
            var result = new ProviderResponse();

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("rrs_bcbsma_view_provider_info", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;

                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                result.IsValid = true;
                                result.ErrorMessage = string.Empty;

                                result.FirstName = DbHelper.NullString(dr[dr.GetOrdinal("First_Name")]);
                                result.LastName = DbHelper.NullString(dr[dr.GetOrdinal("Last_Name")]);
                                result.Address1 = DbHelper.NullString(dr[dr.GetOrdinal("Address_1")]);
                                result.Address2 = DbHelper.NullString(dr[dr.GetOrdinal("Address_2")]);
                                result.City = DbHelper.NullString(dr[dr.GetOrdinal("City")]);
                                result.State = DbHelper.NullString(dr[dr.GetOrdinal("State")]);
                                result.ZipCode = DbHelper.NullString(dr[dr.GetOrdinal("Zip_Code")]);
                                result.ProviderId = DbHelper.NullString(dr[dr.GetOrdinal("Provider_ID")]);
                                result.Suffix = DbHelper.NullString(dr[dr.GetOrdinal("Suffix")]);
                            }
                            else
                            {
                                result.IsValid = false;
                                result.ErrorMessage = "No record exists for provider id = " + providerId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = "Error viewing BCBSMA provider info due to: " + ex.Message;
            }

            return result;
        }

        public static string GetBcbsmaProviderReminder(string providerId, string subscriberId, string memberSeq, DateTime startDate)
        {
            string result;

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("rrs_bcbsma_remind_provider_preauth_visit13", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@pProvider_id", SqlDbType.VarChar)).Value = providerId;
                        cmd.Parameters.Add(new SqlParameter("@pSubscriber_id", SqlDbType.VarChar)).Value = subscriberId;
                        cmd.Parameters.Add(new SqlParameter("@pMember_seq", SqlDbType.VarChar)).Value = memberSeq;
                        cmd.Parameters.Add(new SqlParameter("@pStart_date", SqlDbType.DateTime)).Value = startDate;
                        //output
                        var returnVal = cmd.Parameters.Add("@pReturnVal", SqlDbType.VarChar, 1);
                        returnVal.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        result = DbHelper.NullString(returnVal.Value);
                    }
                }
            }
            catch (Exception)
            {
                result = "N";
            }

            return result;
        }

    }
}
