using cangulo.cicd.Abstractions.Constants;
using cangulo.cicd.domain.Helpers;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Octokit;
using System.IO;
using System.Linq;
using System.Text.Json;
using cangulo.cicd.domain.Extensions;
using cangulo.cicd.domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

internal partial class Build : NukeBuild
{
    private Target CalculateNextReleaseNumber => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(async () =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.VersioningSettings, nameof(CICDFile.VersioningSettings));

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();

            var request = CICDFile.VersioningSettings;

            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = _serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = _serviceProvider.GetRequiredService<IReleaseNumberParser>();

            var commitMsgs = await GetCommitsFromLastMergedPR(prService);

            var commitChoosen = commitMsgs.Last();
            var conventionalCommit = commitParser.ParseConventionalCommit(commitChoosen);

            var releaseType = conventionalCommit.CommitType.ToReleaseType();
            var currentReleaseNumber = releaseNumberParser.Parse(request.CurrentVersion);
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            Logger.Info($"next release Number:{nextReleaseNumber} - Release Type: {releaseType}");
            CICDFile.VersioningSettings.CurrentVersion = nextReleaseNumber.ToString();
        });

    private async Task<IEnumerable<string>> GetCommitsFromLastMergedPR(IPullRequestService prService)
    {
        var ghClient = GetGHClient(GitHubActions);
        var commitMsgs = await prService.GetCommitsFromLastMergedPR(ghClient, GitHubActions);

        ControlFlow.Assert(commitMsgs.Any(), "no commits founds");
        Logger.Info($"Commits Found:{commitMsgs.Count()}");
        commitMsgs
            .ToList()
            .ForEach(Logger.Info);
        return commitMsgs;
    }

    private Target UpdateVersionInFiles => _ => _
        .DependsOn(ParseCICDFile, CalculateNextReleaseNumber)
        .Executes(() =>
        {
            var cicdFilePath = RootDirectory / "cicd.json";
            var content = JsonSerializer.Serialize(CICDFile, SerializerContants.SERIALIZER_OPTIONS);
            File.WriteAllText(cicdFilePath, content);

            // TODO: Update Changelog

            var changeLogService = _serviceProvider.GetRequiredService<IChangeLogService>();
        });

    private Target CreateNewRelease => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);
            var client = ghClient.Repository.Release;

            var request = CICDFile.VersioningSettings;
            var nextVersion = request.CurrentVersion;
            var commitMsgs = await GetCommitsFromLastMergedPR(prService);


            Logger.Info($"Creating Release {nextVersion}");

            var newReleaseData = new NewRelease(nextVersion)
            {
                Name = nextVersion,
                Body = "empty body... for now"
            };

            var releaseCreated = await client.Create(repoOwner, repoName, newReleaseData);
            Logger.Success($"Created release {nextVersion}!");

            foreach (var releaseAsset in request.ReleaseAssets)
            {
                var fileName = Path.GetFileName(releaseAsset);

                var assetData = new ReleaseAssetUpload
                {
                    FileName = fileName,
                    RawData = File.OpenRead(RootDirectory / releaseAsset),
                    ContentType = "application/zip"
                };
                await client.UploadAsset(releaseCreated, assetData);
                Logger.Info($"Asset {fileName} uploaded");
            }
        });

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