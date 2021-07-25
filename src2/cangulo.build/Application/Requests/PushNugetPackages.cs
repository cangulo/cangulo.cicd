using cangulo.build.Application.Requests.Enums;

namespace cangulo.build.Application.Requests
{
    public class PushNugetPackages : CLIRequest
    {
        public string NugetPackagesLocation { get; set; }
        public string TargetNugetRepository { get; set; }
        public AddCommentsToPR CommentToPrRequest { get; set; }
    }

    public class AddCommentsToPR : CLIRequest
    {
        public int PullRequestNumber { get; set; }
        public long RepositoryId { get; set; }
        public static new EnvVar[] EnvVarsRequired = new EnvVar[] { EnvVar.GITHUB_TOKEN };
    }
}