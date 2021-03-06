﻿using System;
using System.Collections.Generic;
using ApacheLogParser.DAL.DataBaseModels.ApacheLogModels;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.DAL.Repositories
{
    /// <summary>
    /// Provides data access layer to ApacheLogs table. 
    /// </summary>
    public interface IApacheLogRepository : IRepository<ApacheLog>
    {
        void BulkInsert(List<ApacheLog> entities);
        List<ClientRequestModel> GetTopClientRequestsByDateRange(DateTime? start, DateTime? end, int countOfClients);
        List<RouteRequestModel> GetTopRouteRequestsByDateRange(DateTime? start, DateTime? end, int countOfRoutes);
        List<ApacheLog> GetTopRequestsByDateRange(DateTime? start, DateTime? end, int offset, int limit);
        List<ApacheLogClientModel> GetApacheLogsWitoutGeolocation();
        void UpdateGeolocationByClient(IEnumerable<ApacheLogClientModel> clients);
    }
}