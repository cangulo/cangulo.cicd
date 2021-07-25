using cangulo.cicd.abstractons.Models;
using System;

namespace cangulo.cicd.domain.Parsers
{
    public interface ICommitParser
    {
        ConventionCommit ParseConventionCommit(string lastCommitMessage);
    }

    public class CommitParser : ICommitParser
    {
        public ConventionCommit ParseConventionCommit(string lastCommitMessage)
        {
            var parts = lastCommitMessage.Split(":", StringSplitOptions.TrimEntries);

            if (!Enum.TryParse(parts[0], out CommitType commitType) || commitType == CommitType.undefined)
                throw new Exception($"commit msg does not provide a valid convention commit type.");

            return new ConventionCommit
            {
                CommitType = commitType,
                Body = parts[1]
            };
        }
    }
}
