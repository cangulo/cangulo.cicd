using cangulo.cicd.abstractons.Models.Enums;

namespace cangulo.cicd.abstractons.Models
{
    // FOLLOWING https://www.conventionalcommits.org/en/v1.0.0/
    public class ConventionCommit
    {
        public CommitType CommitType { get; set; }
        public string Body { get; set; }
    }

}