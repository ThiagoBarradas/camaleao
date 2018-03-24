using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Core.Repository
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAll();
        Task<List<T>> Get(Expression<Func<T, bool>> expression);
        Task Add(T value);
        bool Update(string id, T valeu);
        bool Remove(string id);


    }
}
