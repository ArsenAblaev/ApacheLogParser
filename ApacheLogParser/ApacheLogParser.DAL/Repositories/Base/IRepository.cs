using System.Collections.Generic;
using ApacheLogParser.Entities.Entities;

namespace ApacheLogParser.DAL.Repositories.Base
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        List<TEntity> GetList();
        TEntity Create(TEntity entity);
    }
}
