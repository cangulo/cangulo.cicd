using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.cicd.UT.common
{
    public static class TextFileReader
    {
        public static async Task<string> ReadAsync(string fileRelativePath, CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileRelativePath);
            if (File.Exists(filePath))
            {
                return await File.ReadAllTextAsync(filePath, cancellationToken);
            }
            else
                throw new Exception($"Invalida path provided: {fileRelativePath}");
        }
    }
}
