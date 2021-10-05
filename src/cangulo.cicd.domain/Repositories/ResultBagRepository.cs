using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using cangulo.cicd.abstractions.Constants;
using Nuke.Common.IO;

namespace cangulo.cicd.domain.Repositories
{
    public interface IResultBagRepository
    {
        string GetResult(string key);
        T GetResult<T>(string key) where T : class;
        void AddResult<T>(string key, T value);
    }

    public class ResultBagRepository : IResultBagRepository
    {
        private readonly AbsolutePath ResultBagFilePath;

        public ResultBagRepository(AbsolutePath resultBagFilePath)
        {
            ResultBagFilePath = resultBagFilePath ?? throw new ArgumentNullException(nameof(resultBagFilePath));
        }

        private IDictionary<string, object> GetResultBagDictionary()
        {
            if (File.Exists(ResultBagFilePath))
            {
                var jsonString = File.ReadAllText(ResultBagFilePath);
                return JsonSerializer.Deserialize<IDictionary<string, object>>(jsonString, SerializerContants.DESERIALIZER_OPTIONS);
            }
            else
                return new Dictionary<string, object>();
        }

        public string GetResult(string key)
        {
            var resultBag = GetResultBagDictionary();
            if (resultBag.Keys.Any(x => x == key))
            {
                return resultBag[key].ToString();
            }
            else
                throw new Exception($"{key} not found in the result bag");
        }

        public T GetResult<T>(string key) where T : class
        {
            var resultBag = GetResultBagDictionary();
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

        public void AddResult<T>(string key, T value)
        {
            var resultBag = GetResultBagDictionary();
            resultBag.Add(key, value);

            var jsonString = JsonSerializer.Serialize(resultBag, SerializerContants.SERIALIZER_OPTIONS);
            File.WriteAllText(ResultBagFilePath, jsonString);
        }
    }
}
