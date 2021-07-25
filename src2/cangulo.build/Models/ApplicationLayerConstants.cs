using System.Text.Json;
using System.Text.Json.Serialization;

namespace cangulo.build.Models
{
    public static class ApplicationLayerConstants
    {
        public static class SerializerContants
        {
            public static JsonSerializerOptions SERIALIZER_OPTIONS = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            public static JsonSerializerOptions DESERIALIZER_OPTIONS = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }
    }
}