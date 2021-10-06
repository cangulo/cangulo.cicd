namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class PullRequestSettings
    {
        public bool ValidateConventionalCommits { get; set; }
        public bool ValidateIssueNumber { get; set; }
        public ConventionalCommitsSetting[] ConventionalCommitsSettings { get; set; }
    }
    public class ConventionalCommitsSetting
    {
        public string Type { get; set; }
        public string ReleaseType { get; set; }
    }
}
