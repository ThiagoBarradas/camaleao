using System.Collections.Generic;
using System.Threading.Tasks;
using Camaleao.Core;
using Camaleao.Core.Entities;
using Camaleao.Core.Repository;

namespace Camaleao.Repository
{
    public class ResponseRepository :  BaseRepository<ResponseTemplate>, IResponseRepository
    {
        public ResponseRepository(Settings settings) : base(settings)
        {
        }

        public Task Add(IEnumerable<ResponseTemplate> responses)
        {
            return GetMongoCollection().InsertManyAsync(responses);
        }

        protected override string GetCollectionName()
        {
            return "response";
        }
    }
}
