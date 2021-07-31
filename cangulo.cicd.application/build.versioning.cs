using cangulo.cicd.Abstractions.Constants;
using cangulo.cicd.domain.Extensions;
using cangulo.cicd.domain.Helpers;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Nuke.Common.Tooling;
using Octokit;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target CreateNewRelease => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.VersioningSettings, "versioningSettings should be provided in the cicd.json");

            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = _serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = _serviceProvider.GetRequiredService<IReleaseNumberParser>();

            var currentReleaseNumber = releaseNumberParser.Parse(CICDFile.VersioningSettings.CurrentVersion);

            Logger.Info($"GitRepoInfo:{JsonSerializer.Serialize(GitRepository, SerializerContants.SERIALIZER_OPTIONS)}");

            var lastCommitId = Git($"log --no-merges --format=%H -n 1", logInvocation: false, logOutput: false).Select(x => x.Text).Single();

            Logger.Info($"Last Commit: {lastCommitId} Message:");
            var cmdOutput = Git($"show {lastCommitId} -q --oneline --format=%B", logInvocation: true, logOutput: true);

            ControlFlow.Assert(cmdOutput.All(x => x.Type == OutputType.Std), "error getting last commit");

            var commitMsg = string.Join('\n', cmdOutput.Select(x => x.Text).ToArray());
            ControlFlow.Assert(!string.IsNullOrEmpty(commitMsg), "last commit does not have a msg");

            var conventionCommit = commitParser.ParseConventionCommit(commitMsg);
            Logger.Info($"Commit Type: {conventionCommit.CommitType} ");

            var releaseType = conventionCommit.CommitType.ToReleaseType();
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            var envVars = EnvironmentInfo.Variables;
            envVars.ToList().ForEach(x => Logger.Info($"{x.Key}:{x.Value}"));
            // 
            // 
            // 
            // request.RepositoryId


            //long repositoryId = 330426518;
            //string githubToken = this.GitHubToken;
            //var client = new GitHubClient(new ProductHeaderValue("cangulo.cicd"));
            //client.Credentials = new Credentials(githubToken);

            //var repo = await client.Repository.Get(repositoryId);
        });
}