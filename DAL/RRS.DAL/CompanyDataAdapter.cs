using System.Data;
using System.Data.SqlClient;
using RRS.BEL;

namespace RRS.DAL
{
    public class CompanyDataAdapter
    {
        public static CompanyData GetCompanyData()
        {
            var companyData = new CompanyData();

            using (var conn = new SqlConnection(DbInfo.ConnectionString))
            {
                const string sql = "select isnull(Waiver_Visits,0) as Waiver_Visits, isnull(Waiver_Days,0) as Waiver_Days " +
                                   "from CompanyData";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            companyData.WaiverVisits = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Waiver_Visits")]));
                            companyData.WaiverDays = int.Parse(DbHelper.NullString(dr[dr.GetOrdinal("Waiver_Days")]));
                        }
                    }
                }
            }

            return companyData;
        }
    }
}
