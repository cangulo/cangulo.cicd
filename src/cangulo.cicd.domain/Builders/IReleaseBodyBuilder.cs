using System.Linq;
using System.Text;

namespace cangulo.cicd.domain.Builders
{
    public interface IReleaseBodyBuilder
    {
        public string Build(string[] changes);
    }

    public class ReleaseBodyBuilder : IReleaseBodyBuilder
    {
        public string Build(string[] changes)
        {
            if (changes.Any())
            {
                var body = new StringBuilder();

                changes
                    .ToList()
                    .ForEach(x => body.AppendLine(MarkdownBullet(x)));

                return body.ToString();
            }
            return string.Empty;
        }

        private string MarkdownBullet(string input) => $"* {input}";
    }
}
