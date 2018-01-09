using System.Configuration;

namespace ApacheLogParser.DAL.Repositories.Base
{
    public abstract class BaseRepository
    {
        protected readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["ApacheDB"].ConnectionString;
    }
}
