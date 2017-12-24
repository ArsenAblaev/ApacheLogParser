using System.Collections.Generic;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.DAL.Repositories
{
    public interface IApacheLogRepository : IRepository<ApacheLog>
    {
        void BulkInsert(List<ApacheLog> entities);
    }
}