using System;
using System.Collections.Generic;
using ApacheLogParser.DAL.DataBaseModels.ApacheLogModels;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.DAL.Repositories
{
    public interface IApacheLogRepository : IRepository<ApacheLog>
    {
        void BulkInsert(List<ApacheLog> entities);
        List<ClientRequestModel> GetTopClientRequestsByDateRange(DateTime? start, DateTime? end,int countOfClients);
        List<RouteRequestModel> GetTopRouteRequestsByDateRange(DateTime? start, DateTime? end,int countOfRoutes);
    }
}