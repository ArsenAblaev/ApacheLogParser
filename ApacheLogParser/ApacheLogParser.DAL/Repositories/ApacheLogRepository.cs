using System;
using System.Collections.Generic;
using System.Configuration;
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
                var sqlQuery = "insert into ApacheLogs(Ip,QueryParams,RequestDate,Route,Size,StatusCode) " +
                               " VALUES(@Ip,@QueryParams,@RequestDate,@Route,@Size,@StatusCode); SELECT CAST(SCOPE_IDENTITY() as int)";

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


                    copy.ColumnMappings.Add(0, 1);
                    copy.ColumnMappings.Add(1, 2);
                    copy.ColumnMappings.Add(2, 3);
                    copy.ColumnMappings.Add(3, 4);
                    copy.ColumnMappings.Add(4, 5);
                    copy.ColumnMappings.Add(5, 6);

                    table.Columns.Add("RequestDate", typeof(DateTime));
                    table.Columns.Add("Ip", typeof(string));
                    table.Columns.Add("Route", typeof(string));
                    table.Columns.Add("QueryParams", typeof(string));
                    table.Columns.Add("StatusCode", typeof(short));
                    table.Columns.Add("Size", typeof(int));

                    foreach (var entity in entities)
                    {
                        table.Rows.Add(entity.RequestDate, entity.Ip, entity.Route, entity.QueryParams,
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
