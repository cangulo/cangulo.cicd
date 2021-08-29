using cangulo.changelog.Models;
using cangulo.changelog.Parsers;
using cangulo.changelog.markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cangulo.changelog.Builders.ConventionalCommits
{
    public class ChangesAreaBuilderForConventionalCommits : IChangesListAreaBuilder
    {
        private readonly IConventionalCommitParser _conventionalCommitParser;
        private ChangelogSettings _changelogSettings { get; set; }

        public ChangesAreaBuilderForConventionalCommits(IConventionalCommitParser conventionalCommitParser, ChangelogSettings changelogSettings)
        {
            _conventionalCommitParser = conventionalCommitParser ?? throw new ArgumentNullException(nameof(conventionalCommitParser));
            _changelogSettings = changelogSettings ?? throw new ArgumentNullException(nameof(changelogSettings));
        }

        public string Build(string[] changes)
        {
            var types = _changelogSettings.ConventionalCommitsSettings.Types;
            var conventionalCommits = new List<ConventionalCommit>();

            changes
                .ToList()
                .ForEach(x =>
                {
                    var conventionalCommit = _conventionalCommitParser.Parse(x);
                    conventionalCommits.Add(conventionalCommit);
                });

            var typeVsCommitMsgsList = conventionalCommits
                .GroupBy(x => x.Type)
                .Select(group => new { type = group.Key, msgs = group.Select(x => x.Message) });

            if (typeVsCommitMsgsList.Any())
            {
                var body = new StringBuilder();

                foreach (var typeVsCommitMsgs in typeVsCommitMsgsList)
                {
                    body.AppendLine($"{typeVsCommitMsgs.type}:");

                    var mdListItems = typeVsCommitMsgs
                                            .msgs
                                            .Select(x =>
                                                MarkdownBuilder.ListItem(x, ListItemLevel.Level1));

                    var IsNotLastItem = typeVsCommitMsgs.type != typeVsCommitMsgsList.Last().type;
                    if (IsNotLastItem)
                        mdListItems
                            .ToList()
                            .ForEach(x =>
                                body.AppendLine(x));
                    else
                        body.AppendJoin('\n', mdListItems.ToArray());
                }

                return body.ToString();
            }
            return string.Empty;
        }
    }
}