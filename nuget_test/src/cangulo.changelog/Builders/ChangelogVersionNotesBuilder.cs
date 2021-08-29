using cangulo.changelog.markdown;
using System;
using System.Text;
using static cangulo.changelog.Models.Constants;

namespace cangulo.changelog.Builders
{
    public interface IChangelogVersionNotesBuilder
    {
        string Build(string version, string[] changes);
    }

    public class ChangelogVersionNotesBuilder : IChangelogVersionNotesBuilder
    {
        private readonly IChangesListAreaBuilder _changesListAreaBuilder;

        public ChangelogVersionNotesBuilder(IChangesListAreaBuilder changesListAreaBuilder)
        {
            _changesListAreaBuilder = changesListAreaBuilder ?? throw new ArgumentNullException(nameof(changesListAreaBuilder));
        }

        public string Build(string version, string[] changes)
        {
            var body = new StringBuilder();

            var startVersionTag = MarkdownBuilder.Comment($"{SectionsDelimiterConstants.START_VERSION} {version}");
            body.AppendLine(startVersionTag);

            var versionTitle = MarkdownBuilder.Title(version, TitleLevel.Level1);
            body.AppendLine(versionTitle);

            var dateStr = DateTime.UtcNow.ToString("yyyy-MM-dd");
            body.AppendLine(dateStr);

            body.AppendLine();

            var changesAreas = _changesListAreaBuilder.Build(changes);
            body.AppendLine(changesAreas);

            var endVersionTag = MarkdownBuilder.Comment($"{SectionsDelimiterConstants.END_VERSION} {version}");
            body.Append(endVersionTag);

            return body.ToString();
        }
    }
}