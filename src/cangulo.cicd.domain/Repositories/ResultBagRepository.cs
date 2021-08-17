using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using cangulo.cicd.abstractions.Constants;
using Nuke.Common.IO;

namespace cangulo.cicd.domain.Repositories
{
    public interface IResultBagRepository
    {
        Task<string> GetResult(string key);
        Task<T> GetResult<T>(string key) where T : class;
        Task AddResult<T>(string key, T value);
    }

    public class ResultBagRepository : IResultBagRepository
    {
        private readonly AbsolutePath ResultBagFilePath;

        public ResultBagRepository(AbsolutePath resultBagFilePath)
        {
            ResultBagFilePath = resultBagFilePath ?? throw new ArgumentNullException(nameof(resultBagFilePath));
        }

        private Task<IDictionary<string, object>> GetResultBagDictionaryAsync()
        {
            if (File.Exists(ResultBagFilePath))
            {
                using var openStream = File.OpenRead(ResultBagFilePath);
                return JsonSerializer.DeserializeAsync<IDictionary<string, object>>(openStream, SerializerContants.DESERIALIZER_OPTIONS).AsTask();
            }
            else
                return Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>());
        }

        public async Task<string> GetResult(string key)
        {
            var resultBag = await GetResultBagDictionaryAsync();
            if (resultBag.Keys.Any(x => x == key))
            {
                return resultBag[key].ToString();
            }
            else
                throw new Exception($"{key} not found in the result bag");
        }

        public async Task<T> GetResult<T>(string key) where T : class
        {
            var resultBag = await GetResultBagDictionaryAsync();
            if (resultBag.Keys.Any(x => x == key))
            {
                var resultString = resultBag[key].ToString();
                return JsonSerializer.Deserialize<T>(resultString, SerializerContants.DESERIALIZER_OPTIONS);
            }
            else
            {
                throw new Exception($"{key} not found in the result bag");
            }
        }

        public async Task AddResult<T>(string key, T value)
        {
            var resultBag = await GetResultBagDictionaryAsync();
            resultBag.Add(key, value);

            using var openStream = File.OpenWrite(ResultBagFilePath);
            await JsonSerializer.SerializeAsync(openStream, resultBag, SerializerContants.SERIALIZER_OPTIONS);
        }
    }
}
