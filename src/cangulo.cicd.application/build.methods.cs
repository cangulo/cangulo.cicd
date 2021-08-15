using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Octokit;

internal partial class Build : NukeBuild
{
    private void ValidateCICDPropertyIsProvided(object obj, string propertyName)
        => ControlFlow.NotNull(obj, $"{propertyName} should be provided in the cicd.json");

    private GitHubClient GetGHClient(GitHubActions gitHubAction)
    {
        var repoOwner = GitHubActions.GitHubRepositoryOwner;
        var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);

        // TODO: Migrate the injection of the client to an interface
        var ghClient = new GitHubClient(new ProductHeaderValue($"{repoOwner}"));
        ghClient.Credentials = new Credentials(GitHubToken);

        return ghClient;
    }
}