using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camaleao.Core.Repository
{
    public interface IResponseRepository:IRepository<Response>
    {
        Task Add(IEnumerable<Response> responses);
    }
}
