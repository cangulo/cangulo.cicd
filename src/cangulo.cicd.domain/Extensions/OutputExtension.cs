using Nuke.Common.Tooling;
using System.Collections.Generic;
using System.Linq;

namespace cangulo.cicd.domain.Extensions
{
    public static class OutputExtension
    {
        public static string ConcatenateOutputText(this IReadOnlyCollection<Output> outputList)
        {
            var outputs = outputList
                .Select(x => x.Text)
                .Where(x => !string.IsNullOrEmpty(x));
            return string.Join("\n", outputs.ToArray());
        }
    }
}
