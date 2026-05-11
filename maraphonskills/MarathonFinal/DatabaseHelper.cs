using System.Configuration;
using System.Data.SqlClient;

namespace MarathonFinal
{
    public static class DatabaseHelper
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["MarathonDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
