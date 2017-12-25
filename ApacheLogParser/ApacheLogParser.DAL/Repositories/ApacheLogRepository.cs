using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;
using Dapper;

namespace ApacheLogParser.DAL.Repositories
{
    public class ApacheLogRepository : BaseRepository, IApacheLogRepository
    {
        public List<ApacheLog> GetList()
        {
            List<ApacheLog> cars;

            using (Connection)
            {
                cars = Connection.Query<ApacheLog>("SELECT * FROM ApacheLogs").ToList();
            }

            return cars;
        }

        public ApacheLog Create(ApacheLog log)
        {
            using (Connection)
            {
                var sqlQuery = "INSERT INTO ApacheLogs(Client,QueryParams,RequestDate,Route,Size,StatusCode) " +
                               " VALUES(@Client,@QueryParams,@RequestDate,@Route,@Size,@StatusCode); SELECT CAST(SCOPE_IDENTITY() as int)";

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



                    table.Columns.Add(nameof(ApacheLog.RequestDate), typeof(DateTime));
                    table.Columns.Add(nameof(ApacheLog.Client), typeof(string));
                    table.Columns.Add(nameof(ApacheLog.Route), typeof(string));
                    table.Columns.Add(nameof(ApacheLog.QueryParams), typeof(string));
                    table.Columns.Add(nameof(ApacheLog.StatusCode), typeof(short));
                    table.Columns.Add(nameof(ApacheLog.Size), typeof(int));

                    foreach (var entity in entities)
                    {
                        table.Rows.Add(entity.RequestDate, entity.Client, entity.Route, entity.QueryParams,
                                entity.StatusCode, entity.Size);
                    }

                    copy.WriteToServer(table);
                }
            }

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
