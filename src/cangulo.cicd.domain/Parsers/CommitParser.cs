using cangulo.cicd.abstractions.Models;
using cangulo.cicd.abstractions.Models.Enums;
using Nuke.Common;
using System;
using System.Linq;

namespace cangulo.cicd.domain.Parsers
{
    public interface ICommitParser
    {
        ConventionCommit ParseConventionalCommit(string lastCommitMessage);
    }

    public class CommitParser : ICommitParser
    {
        private const string InvalidCommitMsg = "commit msg does not provide a valid convention commit type.";

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
                Body = parts[1].Trim()
            };
        }
    }
}
