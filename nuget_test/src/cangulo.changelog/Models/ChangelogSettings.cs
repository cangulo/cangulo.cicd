namespace cangulo.changelog.Models
{
    public class ChangelogSettings
    {
        public CommitsMode CommitsMode { get; set; }
        public ConventionalCommitsSettings ConventionalCommitsSettings { get; set; }
    }

    public class ConventionalCommitsSettings
    {
        public string[] Types { get; set; }
    }
}
