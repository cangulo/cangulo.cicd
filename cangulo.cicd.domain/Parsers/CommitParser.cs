using cangulo.cicd.abstractons.Models;
using cangulo.cicd.abstractons.Models.Enums;
using Nuke.Common;
using System;
using System.Linq;

namespace cangulo.cicd.domain.Parsers
{
    public interface ICommitParser
    {
        ConventionCommit[] ParseConventionalCommitFromMergeCommit(string mergeCommit);
        ConventionCommit ParseConventionalCommit(string lastCommitMessage);
        ConventionCommit[] ParseConventionalCommits(string[] lastCommitMessage);
    }

    public class CommitParser : ICommitParser
    {
        private const string InvalidCommitMsg = "commit msg does not provide a valid convention commit type.";

        public ConventionCommit[] ParseConventionalCommitFromMergeCommit(string mergeCommit)
        {
            return mergeCommit
                            .Split("\n", StringSplitOptions.TrimEntries)
                            .ToList()
                            .Skip(1)
                            .Select((x, index) =>
                            {
                                Logger.Info($"commit {index}: {x}");
                                return ParseConventionalCommit(x);
                            }).ToArray();
        }
        public ConventionCommit ParseConventionalCommit(string lastCommitMessage)
        {
            var parts = lastCommitMessage.Split(":", StringSplitOptions.TrimEntries);

            if (parts.Length < 2 || parts.Any(string.IsNullOrEmpty))
                throw new ArgumentException(InvalidCommitMsg);

            if (!Enum.TryParse(parts[0], ignoreCase: true, out CommitType commitType) || commitType == CommitType.Undefined)
                throw new InvalidOperationException(InvalidCommitMsg);

            return new ConventionCommit
            {
                CommitType = commitType,
                Body = parts[1]
            };
        }
        public ConventionCommit[] ParseConventionalCommits(string[] lastCommitMessage)
            => lastCommitMessage
                .Select(ParseConventionalCommit)
                .ToArray();

    }
}
