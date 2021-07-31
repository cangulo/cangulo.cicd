using cangulo.cicd.Abstractions.Constants;
using cangulo.cicd.domain.Extensions;
using cangulo.cicd.domain.Helpers;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tooling;
using Octokit;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target CreateNewRelease => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(async () =>
        {
            ControlFlow.NotNull(CICDFile.VersioningSettings, "versioningSettings should be provided in the cicd.json");

            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = _serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = _serviceProvider.GetRequiredService<IReleaseNumberParser>();

            var currentReleaseNumber = releaseNumberParser.Parse(CICDFile.VersioningSettings.CurrentVersion);

            var lastCommitId = Git($"log --no-merges --format=%H -n 1", logInvocation: false, logOutput: false).Select(x => x.Text).Single();

            Logger.Info($"Last Commit: {lastCommitId} Message:");
            var cmdOutput = Git($"show {lastCommitId} -q --oneline --format=%B", logInvocation: true, logOutput: true);

            ControlFlow.Assert(cmdOutput.All(x => x.Type == OutputType.Std), "error getting last commit");

            var commitMsg = string.Join('\n', cmdOutput.Select(x => x.Text).ToArray());
            ControlFlow.Assert(!string.IsNullOrEmpty(commitMsg), "last commit does not have a msg");

            var conventionCommit = commitParser.ParseConventionCommit(commitMsg);

            var releaseType = conventionCommit.CommitType.ToReleaseType();
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            Logger.Info($"next release Number:{nextReleaseNumber} - Release Type: {releaseType}");


            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");


            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            Logger.Info($"{repoOwner}");
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            Logger.Info($"{repoName}");


            var client = new GitHubClient(new ProductHeaderValue($"{repoOwner}"));
            client.Credentials = new Credentials(GitHubToken);

            Logger.Info($"{repoName} - {repoOwner}");
            var repo = await client.Repository.Get(repoOwner, repoName);

            Logger.Info($"Repo:{JsonSerializer.Serialize(repo, SerializerContants.SERIALIZER_OPTIONS)}");
        });
}