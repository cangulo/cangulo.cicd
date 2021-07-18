namespace cangulo.cicd.abstractons.Models
{
    // FOLLOWING https://www.conventionalcommits.org/en/v1.0.0/
    public class ConventionCommit
    {
        public CommitType CommitType { get; set; }
        public string Body { get; set; }
    }

    public enum CommitType
    {
        undefined,
        fix,
        feat,
        major
    }
}
