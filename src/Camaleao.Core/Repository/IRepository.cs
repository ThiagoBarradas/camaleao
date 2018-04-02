using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Core.Repository
{
    public interface IRepository<TEntity>
    {
        Task<List<TEntity>> GetAll();
        Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter);
        Task Add(TEntity entity);
        bool Update(Expression<Func<TEntity, bool>> filter, TEntity entity);
        bool Remove(Expression<Func<TEntity, bool>> filter);


    }
}
