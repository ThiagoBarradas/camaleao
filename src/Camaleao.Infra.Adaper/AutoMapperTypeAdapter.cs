using AutoMapper;
using Camaleao.Infra.Adaper.Seedwork;

namespace Camaleao.Infra.Adaper {
    public class AutoMapperTypeAdapter : ITypeAdapter {

        public TTarget Adapt<TTarget>(object source) where TTarget : class {
            return Mapper.Map(source, source.GetType(), typeof(TTarget)) as TTarget;
        }

        public TTarget Adapt<TSource, TTarget>(TSource source) where TSource : class where TTarget : class {
            return Mapper.Map<TSource, TTarget>(source);
        }
        public TTarget Adapt<TSource, TTarget>(TSource source, TTarget target) where TSource : class where TTarget : class {
            return Mapper.Map<TSource, TTarget>(source, target);
        }
    }
}
