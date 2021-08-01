using cangulo.cicd.abstractons.Models;
using cangulo.cicd.abstractons.Models.Enums;
using Nuke.Common;
using System;
using System.Linq;

namespace cangulo.cicd.domain.Parsers
{
    public interface ICommitParser
    {
        ConventionCommit[] ParseConventionCommitFromMergeCommit(string mergeCommit);
        ConventionCommit ParseConventionCommit(string lastCommitMessage);
    }

    public class CommitParser : ICommitParser
    {
        private const string InvalidCommitMsg = "commit msg does not provide a valid convention commit type.";

        public ConventionCommit[] ParseConventionCommitFromMergeCommit(string mergeCommit)
        {
            return mergeCommit
                            .Split("\n", StringSplitOptions.TrimEntries)
                            .ToList()
                            .Skip(1)
                            .Select((x, index) =>
                            {
                                Logger.Info($"commit {index}: {x}");
                                return ParseConventionCommit(x);
                            }).ToArray();
        }
        public ConventionCommit ParseConventionCommit(string lastCommitMessage)
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
    }
}
