using Camaleao.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camaleao.Core.Repository
{
    public interface IResponseRepository:IRepository<ResponseTemplate>
    {
        Task Add(IEnumerable<ResponseTemplate> responses);
    }
}
