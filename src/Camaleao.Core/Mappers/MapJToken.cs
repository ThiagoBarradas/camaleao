using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Camaleao.Core.Mappers
{
    public static class MapJToken
    {
        public static  Dictionary<string, dynamic> MapperContract(this JToken request)
        {
            Dictionary<string, dynamic> mapper = new Dictionary<string, dynamic>();
            var children = request.Children<JToken>();

            foreach (var item in children)
            {
                if (item.HasValues && item.First != null)
                {
                    mapper.AddRange(item.MapperContract());
                }
                else
                {
                    mapper.Add(item.Path, item);
                }
            }

            return mapper;
        }

        public static Dictionary<string, dynamic> MapperContractFromObject(this JObject request)
        {
            Dictionary<string, dynamic> mapper = new Dictionary<string, dynamic>();
            var children = request.Children<JToken>();

            foreach (var item in children)
            {
                if (item.HasValues && item.First != null)
                {
                    mapper = item.MapperContract();
                }
                else
                {
                    mapper.Add(item.Path, item);
                }
            }

            return mapper;
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
