using System.Linq;
using cangulo.cicd.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Octokit;

internal partial class Build : NukeBuild
{
    private Target GetCommitsInAMergedPR => _ => _
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var ghClient = new GitHubClient(new ProductHeaderValue($"{repoOwner}"));
            ghClient.Credentials = new Credentials(GitHubToken);
            var client = ghClient.Repository.PullRequest;

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();

            var commitMsgs = await prService.GetCommitsFromLastMergedPR(ghClient, GitHubActions);
            
            ControlFlow.Assert(commitMsgs.Any(), "no commits founds");
            
            Logger.Info($"Commits Found:{commitMsgs.Count()}");
            commitMsgs
                .ToList()
                .ForEach(Logger.Info);
        });
}