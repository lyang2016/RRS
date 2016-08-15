using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace RRS.DAL
{
    public class DiagCodeAdapter
    {
        public static Dictionary<string, string> CheckDiagCode(string code, bool isIvr)
        {
            var dict = new Dictionary<string, string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                if (isIvr)
                {
                    using (var cmd = new SqlCommand("rrs_check_diag_code_ivr", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@diag_ivr", SqlDbType.VarChar)).Value = code;
                        //output
                        var returnVal = cmd.Parameters.Add("@returnval", SqlDbType.Bit, 1);
                        returnVal.Direction = ParameterDirection.Output;
                        var diagCode = cmd.Parameters.Add("@diag_code", SqlDbType.VarChar, 8);
                        diagCode.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        dict["returnval"] = DbHelper.NullString(returnVal.Value);
                        dict["diag_code"] = DbHelper.NullString(diagCode.Value);
                    }                    
                }
                else
                {
                    using (var cmd = new SqlCommand("rrs_check_diag_code", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //output
                        var diagCode = cmd.Parameters.Add("@diag", SqlDbType.VarChar);
                        diagCode.Value = code;
                        diagCode.Direction = ParameterDirection.InputOutput;
                        var returnVal = cmd.Parameters.Add("@returnval", SqlDbType.Bit, 1);
                        returnVal.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        dict["returnval"] = DbHelper.NullString(returnVal.Value);
                        dict["diag_code"] = DbHelper.NullString(diagCode.Value);
                    }                       
                }
            }

            return dict;
        }

        public static string GetDrgCode(string primaryDiagCode, string providerSuffix)
        {
            string result;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand("rrs_ValidDiagnosticCode_Web", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@pDiagnosis_Code", SqlDbType.VarChar)).Value = primaryDiagCode;
                    cmd.Parameters.Add(new SqlParameter("@pProv_Suffix", SqlDbType.VarChar)).Value = providerSuffix;
                    //output
                    var drgCode = cmd.Parameters.Add("@pDRG_Code", SqlDbType.VarChar, 3);
                    drgCode.Direction = ParameterDirection.Output;
                    var errNum = cmd.Parameters.Add("@pError", SqlDbType.Char, 1);
                    errNum.Direction = ParameterDirection.Output;
                    var errMsg = cmd.Parameters.Add("@pError_message", SqlDbType.Char, 50);
                    errMsg.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    result = DbHelper.NullString(drgCode.Value);
                }
            }

            return result;
        }

        public static bool ValidateFriScore(string friScore)
        {
            var result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as fri_count from FRIscores " +
                                   "where FRI_value=@FRI_value and Valid=1";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@FRI_value", SqlDbType.VarChar)).Value = friScore;

                    conn.Open();
                    var friCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (friCount > 0)
                        result = true;
                }
            }

            return result;
        }

        public static bool ValidatePsfsScore(string psfsScore)
        {
            var result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as psfs_count from PSFSscores " +
                                   "where PSFS_value=@PSFS_value and Valid=1";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PSFS_value", SqlDbType.VarChar)).Value = psfsScore;

                    conn.Open();
                    var psfsCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (psfsCount > 0)
                        result = true;
                }
            }

            return result;
        }

        public static bool ValidateMedicareDiagCode(string code, int codeType)
        {
            var result = false;

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select count(*) as diag_count from DiagnosticCode_MAM " +
                                   "where Diagnostic_Code=@pDiagnostic_Code and Type=@pType and Valid=1";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pDiagnostic_Code", SqlDbType.VarChar)).Value = code;
                    cmd.Parameters.Add(new SqlParameter("@pType", SqlDbType.Int)).Value = codeType;

                    conn.Open();
                    var diagCount = int.Parse(DbHelper.NullString(cmd.ExecuteScalar()));
                    if (diagCount > 0)
                        result = true;
                }
            }

            return result;
        }

        public static List<string> GetDiagnosticCodes(string keyWord)
        {
            var result = new List<string>();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                var sql = "SELECT DISTINCT diagnostic_code from diagnosticcode " +
                          "where valid=1 and diagnostic_code LIKE '" + keyWord + "%'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(DbHelper.NullString(dr[dr.GetOrdinal("diagnostic_code")]));
                        }
                    }
                }
            }            

            return result;
        }

        public static bool InsertInvalidDiagCode(string code, string providerId)
        {
            var result = true;

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("LogInvalidDiagCode", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Diagnostic_Code", SqlDbType.VarChar)).Value = code;
                        cmd.Parameters.Add(new SqlParameter("@Provider_ID", SqlDbType.VarChar)).Value = providerId;

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
    }
}
