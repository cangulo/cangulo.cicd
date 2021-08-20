using cangulo.cicd.abstractions.Constants;
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
using cangulo.cicd.domain.Builders;
using System;
using cangulo.cicd.domain.Repositories;

internal partial class Build : NukeBuild
{
    private Target CalculateNextReleaseNumber => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(async () =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.VersioningSettings, nameof(CICDFile.VersioningSettings));
            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

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

            var resultKey = nameof(CalculateNextReleaseNumber);
            resultBagRepository.AddResult(resultKey, nextReleaseNumber.ToString());
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
        .Executes(async () =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));
            CICDFile.VersioningSettings.CurrentVersion = nextReleaseNumber;

            using var openStreamCICD = File.OpenWrite(CICDFilePath);
            await JsonSerializer.SerializeAsync(openStreamCICD, CICDFile, SerializerContants.SERIALIZER_OPTIONS);

            // TODO: Update Changelog
        });

    private Target CreateNewRelease => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
            var releaseBodyBuilder = _serviceProvider.GetRequiredService<IReleaseBodyBuilder>();

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);
            var releaseOperatorClient = ghClient.Repository.Release;

            var request = CICDFile.VersioningSettings;
            var nextVersion = request.CurrentVersion;
            var commitMsgs = await GetCommitsFromLastMergedPR(prService);

            Logger.Info($"Creating Release {nextVersion}");

            var newReleaseData = new NewRelease(nextVersion)
            {
                Name = nextVersion,
                Body = releaseBodyBuilder.Build(commitMsgs.ToArray())
            };

            var releaseCreated = await releaseOperatorClient.Create(repoOwner, repoName, newReleaseData);
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
                await releaseOperatorClient.UploadAsset(releaseCreated, assetData);
                Logger.Info($"Asset {fileName} uploaded");
            }
        });
}