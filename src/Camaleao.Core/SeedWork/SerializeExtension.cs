using Newtonsoft.Json;
using System.IO;

namespace Camaleao.Core.SeedWork {
    public static class SerializeExtension {

        public static T DeserializeJson<T>(this Stream stream) {

            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr)) {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}
