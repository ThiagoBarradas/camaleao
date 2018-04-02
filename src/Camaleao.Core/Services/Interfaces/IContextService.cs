using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IContextService
    {
        Context GetContext(string contextKey);
        void Add(Context context);
        void Update(Context context);
    }
}
