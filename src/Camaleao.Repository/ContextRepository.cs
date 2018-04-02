using Camaleao.Core;
using Camaleao.Core.Entities;
using Camaleao.Core.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Repository
{
    public class ContextRepository : BaseRepository<Context>, IContextRepository
    {
        public ContextRepository(Settings settings) : base(settings)
        {
        }

        protected override string GetCollectionName()
        {
            return "context";
        }
    }
}
