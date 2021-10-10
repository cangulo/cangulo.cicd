using cangulo.cicd.abstractions.Models.Enums;

namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class PullRequestSettings
    {
        public ConventionalCommitsSetting[] ConventionalCommitsSettings { get; set; }
        public string IssueNumberRegex { get; set; }
        public bool OutputCommits { get; set; }
        public string OutputFilePath { get; set; } = "commits.txt";
    }
    public class ConventionalCommitsSetting
    {
        public string Type { get; set; }
        public ReleaseType ReleaseType { get; set; }
    }
}
