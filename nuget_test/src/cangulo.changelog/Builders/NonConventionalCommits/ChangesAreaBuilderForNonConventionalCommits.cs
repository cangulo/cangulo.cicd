using cangulo.changelog.markdown;
using System.Linq;
using System.Text;

namespace cangulo.changelog.Builders.NonConventionalCommits
{
    public class ChangesAreaBuilderForNonConventionalCommits : IChangesListAreaBuilder
    {
        public string Build(string[] changes)
        {
            if (changes.Any())
            {
                var body = new StringBuilder();

                var mdListItems = changes
                                    .Select(x =>
                                        MarkdownBuilder.ListItem(x, ListItemLevel.Level1));

                body.AppendJoin("\r\n", mdListItems.ToArray());

                return body.ToString();
            }

            return string.Empty;
        }
    }
}