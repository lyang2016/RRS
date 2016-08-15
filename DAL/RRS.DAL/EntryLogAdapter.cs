using System;
using System.Data;
using System.Data.SqlClient;

namespace RRS.DAL
{
    public class EntryLogAdapter
    {
        public static bool InsertEntryLog(string callStarted, string sessionId, string phoneNumber, string variableName,
                                 string entryString, string validationResult, string isBcbsma)
        {
            var result = true;

            if (validationResult == "")
                validationResult = "False";

            if (isBcbsma == "")
                isBcbsma = "False";

            try
            {
                using (var conn = new SqlConnection(DbInfo.ConnectionString))
                {
                    using (var cmd = new SqlCommand("RRS_Entry_Log_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@CallStarted", SqlDbType.DateTime)).Value = DateTime.Parse(callStarted);
                        cmd.Parameters.Add(new SqlParameter("@SessionID", SqlDbType.VarChar)).Value = sessionId;
                        cmd.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.VarChar)).Value = phoneNumber;
                        cmd.Parameters.Add(new SqlParameter("@VariableName", SqlDbType.VarChar)).Value = variableName;
                        cmd.Parameters.Add(new SqlParameter("@EntryString", SqlDbType.VarChar)).Value = entryString;
                        cmd.Parameters.Add(new SqlParameter("@ValidationResult", SqlDbType.Bit)).Value = Convert.ToBoolean(validationResult);
                        cmd.Parameters.Add(new SqlParameter("@IsBCBSMA", SqlDbType.VarChar)).Value = Convert.ToBoolean(isBcbsma);

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
    }
}
