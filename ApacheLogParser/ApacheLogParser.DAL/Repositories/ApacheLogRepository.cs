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
    public class ApacheLogRepository : BaseRepository, IApacheLogRepository
    {
        public List<ApacheLog> GetList()
        {
            List<ApacheLog> logs;

            using (Connection)
            {
                logs = Connection.Query<ApacheLog>("SELECT * FROM ApacheLogs").ToList();
            }

            return logs;
        }

        public ApacheLog Create(ApacheLog log)
        {
            using (Connection)
            {
                var sqlQuery = "INSERT INTO ApacheLogs(Client,QueryParams,RequestDate,Route,Size,StatusCode,Geolocation) " +
                               " VALUES(@Client,@QueryParams,@RequestDate,@Route,@Size,@StatusCode,@Geolocation); SELECT CAST(SCOPE_IDENTITY() as int)";

                int? userId = Connection.Query<int>(sqlQuery, log).FirstOrDefault();
                log.Id = (int)userId;
            }
            return log;
        }

        public void BulkInsert(List<ApacheLog> entities)
        {
            using (Connection)
            {
                Connection.Open();

                using (var copy = new SqlBulkCopy((SqlConnection)Connection))
                {
                    copy.DestinationTableName = "ApacheLogs";
                    var table = new DataTable("ApacheLogs");

                    //to reduce error chanses.
                    //copy.BatchSize = entities.Count / 10;

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

        public List<ClientRequestModel> GetTopClientRequestsByDateRange(DateTime? start, DateTime? end, int countOfClients)
        {

            List<ClientRequestModel> logs;

            using (Connection)
            {
                logs = Connection.Query<ClientRequestModel>(@"select top (@CountOfClients) Client, COUNT(Client) as CountOfRequests from ApacheLogs
                                                     where RequestDate between ISNULL(@Start,(select MIN(RequestDate) from ApacheLogs))
                                                     and ISNULL(@End,(select MAX(RequestDate) from ApacheLogs)) 
                                                     group by(Client)
                                                     order by COUNT(Client) desc", new { Start = start, End = end, CountOfClients = countOfClients }).ToList();
            }

            return logs;
        }

        public List<RouteRequestModel> GetTopRouteRequestsByDateRange(DateTime? start, DateTime? end, int countOfRoutes)
        {
            List<RouteRequestModel> logs;

            using (Connection)
            {
                logs = Connection.Query<RouteRequestModel>(@"select top (@CountOfRoutes) Route, COUNT(Route) as CountOfRequests from ApacheLogs
                                                     where RequestDate between ISNULL(@Start,(select MIN(RequestDate) from ApacheLogs)) 
                                                     and ISNULL(@End,(select MAX(RequestDate) from ApacheLogs))
                                                     group by(Route)
                                                     order by COUNT(Route) desc", new { Start = start, End = end, CountOfRoutes = countOfRoutes }).ToList();
            }

            return logs;
        }


        public void Update(ApacheLog log)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
