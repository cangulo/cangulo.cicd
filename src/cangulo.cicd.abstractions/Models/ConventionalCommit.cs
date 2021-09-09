using cangulo.cicd.abstractions.Models.Enums;

namespace cangulo.cicd.abstractions.Models
{
    // FOLLOWING https://www.conventionalcommits.org/en/v1.0.0/
    public class ConventionalCommit
    {
        public CommitType CommitType { get; set; }
        public string Body { get; set; }

        public override string ToString()
            => $"{CommitType}: {Body}";
    }
}