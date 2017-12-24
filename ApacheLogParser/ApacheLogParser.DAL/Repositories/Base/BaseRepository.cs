using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ApacheLogParser.DAL.Repositories.Base
{
    public abstract class BaseRepository
    {
        protected readonly IDbConnection Connection;

        protected BaseRepository()
        {
            Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApacheDB"].ConnectionString);
        }
    }
}
