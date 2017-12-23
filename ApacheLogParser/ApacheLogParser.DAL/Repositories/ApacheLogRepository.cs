using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ApacheLogParser.DAL.Repositories.Base;
using ApacheLogParser.Entities.Entities;
using Dapper;

namespace ApacheLogParser.DAL.Repositories
{
    public class ApacheLogRepository : IRepository<ApacheLog>
    {
        private readonly IDbConnection _dataBase;

        public ApacheLogRepository(IDbConnection dataBase)
        {
            _dataBase = dataBase;
        }

        public IList<ApacheLog> GetList()
        {
            IList<ApacheLog> cars;

            using (_dataBase)
            {
                cars = _dataBase.Query<ApacheLog>("SELECT * FROM ApacheLogs").ToList();
            }

            return cars;
        }

        public ApacheLog Get(int id)
        {
            throw new NotImplementedException();
        }

        public ApacheLog Create(ApacheLog user)
        {
            throw new NotImplementedException();
        }

        public void Update(ApacheLog user)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
