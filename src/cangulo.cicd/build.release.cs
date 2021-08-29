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
using System.Text.RegularExpressions;

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

    private Target UpdateVersionInCICDFile => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));
            CICDFile.Versioning.CurrentVersion = nextReleaseNumber;

            var newCICDFileContent = JsonSerializer.Serialize(CICDFile, SerializerContants.SERIALIZER_OPTIONS);

            using StreamWriter fileWriter = new(CICDFilePath, append: false);
            fileWriter.Write(newCICDFileContent);
        });

    private Target UpdateChangelog => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var changelogBuilder = _serviceProvider.GetRequiredService<IChangelogBuilder>();

            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));
            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));

            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");

            changelogBuilder.Build(nextReleaseNumber, commitMsgs, ChangelogPath);
        });
    private Target UpdateReleaseVersionInCSProj => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.Versioning.UpdateVersionInCSProjSettings, "This Target can't be executed locally");

            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));

            var projectPath = CICDFile.Versioning.UpdateVersionInCSProjSettings.ProjectPath;

            using var reader = new StreamReader(projectPath);
            var csprojContent = reader.ReadToEnd();
            reader.Close();

            var pattern = @"<Version>(.*)<\/Version>";
            var newVersionText = $"<Version>{nextReleaseNumber}</Version>";
            var newContent = Regex.Replace(csprojContent, pattern, newVersionText);

            using var writer = new StreamWriter(projectPath);
            writer.Write(newContent);
            writer.Close();
        });

    private Target UpdatePreReleaseVersionInCSProj => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.Versioning.UpdateVersionInCSProjSettings, "This Target can't be executed locally");
            ControlFlow.NotEmpty(CICDFile.Versioning.UpdateVersionInCSProjSettings.PreReleaseVersionSuffix, "Please provide a version suffix in the cicd.json file");

            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));

            var request = CICDFile.Versioning.UpdateVersionInCSProjSettings;
            var projectPath = request.ProjectPath;
            var versionSuffix = request.PreReleaseVersionSuffix;

            using var reader = new StreamReader(projectPath);
            var csprojContent = reader.ReadToEnd();
            reader.Close();

            var pattern = @"<Version>(.*)<\/Version>";
            var newVersionText = $"<Version>{nextReleaseNumber}-{versionSuffix}</Version>";
            var newContent = Regex.Replace(csprojContent, pattern, newVersionText);

            using var writer = new StreamWriter(projectPath);
            writer.Write(newContent);
            writer.Close();
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

            if (request.ReleaseAssets is not null && request.ReleaseAssets.Any())
            {
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
            }
        });
}