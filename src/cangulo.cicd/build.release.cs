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
using cangulo.changelog.builders;
using cangulo.cicd.domain.Repositories;

internal partial class Build : NukeBuild
{
    private Target CalculateNextReleaseNumber => _ => _
        .DependsOn(ListCommitsInThisPR)
        .Executes(() =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = _serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = _serviceProvider.GetRequiredService<IReleaseNumberParser>();

            ValidateCICDPropertyIsProvided(CICDFile.Versioning, nameof(CICDFile.Versioning));
            var request = CICDFile.Versioning;

            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));
            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");

            var commitChosen = commitMsgs.Last();
            var conventionalCommit = commitParser.ParseConventionalCommit(commitChosen);

            var releaseType = conventionalCommit.CommitType.ToReleaseType();
            var currentReleaseNumber = releaseNumberParser.Parse(request.CurrentVersion);
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            Logger.Info($"next release Number:{nextReleaseNumber} - Release Type: {releaseType}");

            var resultKey = nameof(CalculateNextReleaseNumber);
            resultBagRepository.AddResult(resultKey, nextReleaseNumber.ToString());
        });

    private Target UpdateVersionInFiles => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Executes(async () =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var changelogBuilder = _serviceProvider.GetRequiredService<IChangelogBuilder>();

            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));
            CICDFile.Versioning.CurrentVersion = nextReleaseNumber;

            using var openStreamCICD = File.OpenWrite(CICDFilePath);
            await JsonSerializer.SerializeAsync(openStreamCICD, CICDFile, SerializerContants.SERIALIZER_OPTIONS);

            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));
            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");
            changelogBuilder.Build(nextReleaseNumber, commitMsgs, ChangelogPath);
        });

    private Target CreateNewRelease => _ => _
        .DependsOn(ListCommitsInThisPR)
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
            var releaseBodyBuilder = _serviceProvider.GetRequiredService<IReleaseNotesBuilder>();
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);
            var releaseOperatorClient = ghClient.Repository.Release;

            var request = CICDFile.Versioning;
            var nextVersion = request.CurrentVersion;
            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));
            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");

            Logger.Info($"Creating Release {nextVersion}");

            var newReleaseData = new NewRelease(nextVersion)
            {
                Name = nextVersion,
                Body = releaseBodyBuilder.Build(commitMsgs.ToArray())
            };

            var releaseCreated = await releaseOperatorClient.Create(repoOwner, repoName, newReleaseData);
            Logger.Success($"Release {nextVersion} created!");

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