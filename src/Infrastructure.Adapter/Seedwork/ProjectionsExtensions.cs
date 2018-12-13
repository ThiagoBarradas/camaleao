using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Infrastructure.Adapter.Seedwork {
    public static class ProjectionsExtensions {

        /// <summary>
        /// Project a type using a DTO
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <param name="entity">The source entity to project</param>
        /// <returns>The projected type</returns>
        public static TProjection ProjectedAs<TProjection>(this object item) where TProjection : class {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(item);
        }

        public static TProjection ProjectedAs<TSource, TProjection>(this TSource item) where TProjection : class where TSource : class {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TSource, TProjection>(item);
        }

        public static TProjection ProjectedAs<TSource, TProjection>(this TSource item, TProjection projection) where TProjection : class where TSource : class {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TSource, TProjection>(item, projection);
        }
        /// <summary>
        /// projected a enumerable collection of items
        /// </summary>
        /// <typeparam name="TProjection">The dtop projection type</typeparam>
        /// <param name="items">the collection of entity items</param>
        /// <returns>Projected collection</returns>
        public static List<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable<object> items) where TProjection : class {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(items);
        }

        public static List<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable items) where TProjection : class {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(items);
        }
    }
}
