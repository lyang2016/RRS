using System.Data;
using System.Data.SqlClient;

namespace RRS.DAL
{
    public class QuestionsAdapter
    {
        public static DataSet GetQuestions(string questionSet)
        {
            var ds = new DataSet();
            const string sql = "SELECT Var_Name, Data_Type, Max_Length, Validation FROM vw_RRS_Questions " +
                               "WHERE Question_Set=@Question_Set ORDER BY Question_Order";

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Question_Set", SqlDbType.VarChar)).Value = questionSet;
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "RRS_Questions");                        
                    }
                }
            }

            return ds;
        }
    }
}
