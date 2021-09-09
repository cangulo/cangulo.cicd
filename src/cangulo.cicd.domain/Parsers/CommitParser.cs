using cangulo.cicd.abstractions.Models;
using cangulo.cicd.abstractions.Models.Enums;
using Nuke.Common;
using System;
using System.Linq;

namespace cangulo.cicd.domain.Parsers
{
    public interface ICommitParser
    {
        ConventionalCommit ParseConventionalCommit(string commitMsg);
    }

    public class CommitParser : ICommitParser
    {
        public ConventionalCommit ParseConventionalCommit(string commitMsg)
        {
            var parts = commitMsg.Split(":", StringSplitOptions.TrimEntries);

            if (parts.Length < 2 || parts.Any(string.IsNullOrEmpty))
                throw new ArgumentException(BuildErrorMsg(commitMsg));

            if (!Enum.TryParse(parts[0], ignoreCase: true, out CommitType commitType) || commitType == CommitType.Undefined)
                throw new InvalidOperationException(BuildErrorMsg(commitMsg));

            return new ConventionalCommit
            {
                CommitType = commitType,
                Body = parts[1].Trim()
            };
        }

        private string BuildErrorMsg(string commitMsg)
            => $"commit msg does not provide a valid convention commit type. Commit: {commitMsg}";
    }
}
