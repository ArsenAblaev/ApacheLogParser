using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ApacheLogParser.DAL.DataBaseModels.ApacheLogModels;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;
using Dapper;

namespace ApacheLogParser.DAL.Repositories
{
    /// <summary>
    /// Provides data access layer to ApacheLogs table. 
    /// </summary>
    public class ApacheLogRepository : BaseRepository, IApacheLogRepository
    {

        /// <summary>
        /// Get all ApacheLogs from DB
        /// </summary>
        /// <returns></returns>
        public List<ApacheLog> GetList()
        {
            List<ApacheLog> logs;

            using (var connection = new SqlConnection(ConnectionString))
            {
                logs = connection.Query<ApacheLog>("SELECT * FROM ApacheLogs").ToList();
            }

            return logs;
        }

        /// <summary>
        /// Create single ApacheLog
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public ApacheLog Create(ApacheLog log)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {

                var sqlQuery = "INSERT INTO ApacheLogs(Client,QueryParams,RequestDate,Route,Size,StatusCode,Geolocation) " +
                               " VALUES(@Client,@QueryParams,@RequestDate,@Route,@Size,@StatusCode,@Geolocation); SELECT CAST(SCOPE_IDENTITY() as int)";

                int? logId = connection.Query<int>(sqlQuery, log).FirstOrDefault();
                log.Id = (int)logId;
            }

            return log;
        }

        /// <summary>
        /// Get top N-clients ordered by number of requests descending by data range.
        /// If start = null : start = MIN date
        /// If end = null: end = MAX date 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="countOfClients"></param>
        /// <returns></returns>
        public List<ClientRequestModel> GetTopClientRequestsByDateRange(DateTime? start, DateTime? end, int countOfClients)
        {

            List<ClientRequestModel> logs;

            using (var connection = new SqlConnection(ConnectionString))
            {
                logs = connection.Query<ClientRequestModel>(@"select top (@CountOfClients) Client, COUNT(Client) as CountOfRequests from ApacheLogs
                                                     where RequestDate between ISNULL(@Start,(select MIN(RequestDate) from ApacheLogs))
                                                     and ISNULL(@End,(select MAX(RequestDate) from ApacheLogs)) 
                                                     group by(Client)
                                                     order by COUNT(Client) desc", new { Start = start, End = end, CountOfClients = countOfClients }).ToList();
            }

            return logs;
        }

        /// <summary>
        /// Get top N-routes ordered by number of requests descending by date range.
        /// If start = null: start = MIN date
        /// IF end = null: end = MAX date
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="countOfRoutes"></param>
        /// <returns></returns>
        public List<RouteRequestModel> GetTopRouteRequestsByDateRange(DateTime? start, DateTime? end, int countOfRoutes)
        {
            List<RouteRequestModel> logs;

            using (var connection = new SqlConnection(ConnectionString))
            {
                logs = connection.Query<RouteRequestModel>(@"select top (@CountOfRoutes) Route, COUNT(Route) as CountOfRequests from ApacheLogs
                                                     where RequestDate between ISNULL(@Start,(select MIN(RequestDate) from ApacheLogs)) 
                                                     and ISNULL(@End,(select MAX(RequestDate) from ApacheLogs))
                                                     group by(Route)
                                                     order by COUNT(Route) desc", new { Start = start, End = end, CountOfRoutes = countOfRoutes }).ToList();
            }

            return logs;
        }

        /// <summary>
        /// Get all ApacheLog fields ordered by RequestDate with offset and limit
        /// If start = null: start = MIN date
        /// If end = null: end = MAX date
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<ApacheLog> GetTopRequestsByDateRange(DateTime? start, DateTime? end, int offset, int limit)
        {
            List<ApacheLog> logs;

            using (var connection = new SqlConnection(ConnectionString))
            {
                logs = connection.Query<ApacheLog>(@"select * from ApacheLogs
                                                     where RequestDate between ISNULL(@Start,(select MIN(RequestDate) from ApacheLogs)) 
                                                     and ISNULL(@End,(select MAX(RequestDate) from ApacheLogs))
                                                     order by RequestDate
                                                     offset @Offset rows
                                                     fetch next @Limit rows only", new { Start = start, End = end, Offset = offset, Limit = limit }).ToList();
            }

            return logs;
        }

        /// <summary>
        /// Get all unique ApacheLog clients which have not been geolocated.
        /// </summary>
        /// <returns></returns>
        public List<ApacheLogClientModel> GetApacheLogsWitoutGeolocation()
        {
            List<ApacheLogClientModel> logs;

            using (var connection = new SqlConnection(ConnectionString))
            {
                logs = connection.Query<ApacheLogClientModel>("SELECT DISTINCT Client FROM ApacheLogs WHERE Geolocation IS NULL").ToList();
            }

            return logs;
        }


        /// <summary>
        /// Update geolocation for clients.
        /// </summary>
        /// <param name="clients"></param>
        public void UpdateGeolocationByClient(IEnumerable<ApacheLogClientModel> clients)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var clientGeolocationTable = new DataTable();
                clientGeolocationTable.Columns.Add(new DataColumn("Client", typeof(string)));
                clientGeolocationTable.Columns.Add(new DataColumn("Geolocation", typeof(string)));

                foreach (var client in clients)
                {
                    clientGeolocationTable.Rows.Add(client.Client, client.Geolocation);
                }

                connection.Query(@"update [log] set [log].Geolocation = cg.Geolocation
                                 from ApacheLogs [log]
                                 join @ClientGeolocation cg on cg.Client = [log].Client", 
                                 new { ClientGeolocation = clientGeolocationTable.AsTableValuedParameter("dbo.[ClientGeolocation]") });

            }
        }

        /// <summary>
        /// Insert ApacheLogs by bulk insert.
        /// </summary>
        /// <param name="entities"></param>
        public void BulkInsert(List<ApacheLog> entities)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var copy = new SqlBulkCopy(connection))
                {
                    copy.DestinationTableName = "ApacheLogs";
                    var table = new DataTable("ApacheLogs");
                    
                    copy.ColumnMappings.Add(nameof(ApacheLog.RequestDate), nameof(ApacheLog.RequestDate));
                    copy.ColumnMappings.Add(nameof(ApacheLog.Client), nameof(ApacheLog.Client));
                    copy.ColumnMappings.Add(nameof(ApacheLog.Route), nameof(ApacheLog.Route));
                    copy.ColumnMappings.Add(nameof(ApacheLog.QueryParams), nameof(ApacheLog.QueryParams));
                    copy.ColumnMappings.Add(nameof(ApacheLog.StatusCode), nameof(ApacheLog.StatusCode));
                    copy.ColumnMappings.Add(nameof(ApacheLog.Size), nameof(ApacheLog.Size));
                    copy.ColumnMappings.Add(nameof(ApacheLog.Geolocation), nameof(ApacheLog.Geolocation));



                    table.Columns.Add(nameof(ApacheLog.RequestDate), typeof(DateTime));
                    table.Columns.Add(nameof(ApacheLog.Client), typeof(string));
                    table.Columns.Add(nameof(ApacheLog.Route), typeof(string));
                    table.Columns.Add(nameof(ApacheLog.QueryParams), typeof(string));
                    table.Columns.Add(nameof(ApacheLog.StatusCode), typeof(short));
                    table.Columns.Add(nameof(ApacheLog.Size), typeof(int));
                    table.Columns.Add(nameof(ApacheLog.Geolocation), typeof(string));

                    foreach (var entity in entities)
                    {
                        table.Rows.Add(entity.RequestDate, entity.Client, entity.Route, entity.QueryParams,
                                entity.StatusCode, entity.Size);
                    }

                    copy.WriteToServer(table);
                }
            }

        }
    }
}
