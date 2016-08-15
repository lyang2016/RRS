
namespace RRS.DAL
{
    public class DbInfo
    {
        public static string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["RRSConnectionString"].ConnectionString;
            }
        }
    }
}
