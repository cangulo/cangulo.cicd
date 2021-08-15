using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.cicd.UT.common
{
    public static class JsonTestDataParser
    {
        public static async Task<T> DeserializeFile<T>(string fileRelativePath, CancellationToken cancellationToken = default) where T : class
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileRelativePath);
            if (File.Exists(filePath))
            {
                using var openStream = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<T>(openStream, SerializerContants.DESERIALIZER_OPTIONS, cancellationToken) ?? throw new Exception($"Error parsing file{fileRelativePath}");
            }
            else
                throw new Exception($"Invalida path provided: {fileRelativePath}");
        }

        private static class SerializerContants
        {
            public static JsonSerializerOptions SERIALIZER_OPTIONS = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            public static JsonSerializerOptions DESERIALIZER_OPTIONS = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
