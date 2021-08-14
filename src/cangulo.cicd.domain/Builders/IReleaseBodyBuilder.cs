using System.Linq;
using System.Text;

namespace cangulo.cicd.domain.Builders
{
    public interface IReleaseBodyBuilder
    {
        public string Build(string[] changes, string version);
    }

    public class ReleaseBodyBuilder : IReleaseBodyBuilder
    {
        public string Build(string[] changes, string version)
        {
            if (changes.Any() && !string.IsNullOrEmpty(version))
            {
                var body = new StringBuilder();

                body.AppendLine($"<!-- START:{version} -->");
                body.AppendLine($"# {version}\n");

                changes
                    .ToList()
                    .ForEach(x => body.AppendLine(x));

                body.AppendLine($"<!-- END:{version} -->\n");

                return body.ToString();
            }
            return string.Empty;
        }
    }
}
