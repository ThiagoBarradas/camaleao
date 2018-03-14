using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Repository
{
    public interface IRepository<T>
    {
        Task<T> GetAll();
        Task<T> Get(Expression<Func<T, bool>> expression);
        Task Add(T value);
        Task<bool> Update(string id, T valeu);
        Task<bool> Remove(string id);


    }
}
