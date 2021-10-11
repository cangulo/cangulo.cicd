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
using cangulo.cicd.domain.Services;
using cangulo.cicd.domain.Repositories;
using System.Text.RegularExpressions;
using cangulo.changelog.builders;
using cangulo.cicd.abstractions.Models.CICDFile;
using cangulo.cicd.abstractions.Models;

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

            ValidateCICDPropertyIsProvided(CICDFile.ReleaseSettings, nameof(CICDFile.ReleaseSettings));
            var request = CICDFile.ReleaseSettings;

            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));
            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");

            var commitChosen = commitMsgs.Last();
            var commitTypesAllowed = CICDFile
                           .PullRequestSettings
                           .ConventionalCommitsSettings
                           .Select(x => x.Type)
                           .ToArray();

            var conventionalCommit = commitParser.ParseConventionalCommit(commitChosen, commitTypesAllowed);

            var releaseType = CICDFile
                                .PullRequestSettings
                                .ConventionalCommitsSettings
                                .Single(x =>
                                    x.Type.Trim().ToLowerInvariant() == conventionalCommit.CommitType)
                                .ReleaseType;

            var currentReleaseNumber = ReadVersionFromFile(releaseNumberParser);
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            Logger.Info($"next release Number:{nextReleaseNumber} - Release Type: {releaseType}");

            var resultKey = nameof(CalculateNextReleaseNumber);
            resultBagRepository.AddResult(resultKey, nextReleaseNumber.ToString());

            Logger.Success($"saved next release number ({nextReleaseNumber}) in {ResultBagFilePath}");
        });

    private Target UpdateVersionInCICDFile => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));

            var filePath = CICDFile.ReleaseSettings.CurrentVersionFilePath;
            var fileContent = File.ReadAllText(filePath);
            var currentVersionFileModel = JsonSerializer.Deserialize<CurrentVersionFileModel>(fileContent, SerializerContants.DESERIALIZER_OPTIONS);

            currentVersionFileModel.CurrentVersion = nextReleaseNumber;

            var newCurrentVersionJson = JsonSerializer.Serialize(currentVersionFileModel, SerializerContants.SERIALIZER_OPTIONS);

            using StreamWriter fileWriter = new(filePath, append: false);
            fileWriter.Write(newCurrentVersionJson);

            Logger.Success($"updated current version ({nextReleaseNumber}) in {filePath }");
        });

    private Target UpdateReleaseVersionInCSProj => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings, "This Target can't be executed locally");

            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));

            var projectPath = CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings.ProjectPath;

            using var reader = new StreamReader(projectPath);
            var csprojContent = reader.ReadToEnd();
            reader.Close();

            var pattern = @"<Version>(.*)<\/Version>";
            var newVersionText = $"<Version>{nextReleaseNumber}</Version>";
            var newContent = Regex.Replace(csprojContent, pattern, newVersionText);

            using var writer = new StreamWriter(projectPath);
            writer.Write(newContent);
            writer.Close();

            Logger.Success($"updated release version {nextReleaseNumber} in csproj file {projectPath}");
        });

    private Target UpdatePreReleaseVersionInCSProj => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings, "This Target can't be executed locally");
            ControlFlow.NotEmpty(CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings.PreReleaseVersionSuffix, "Please provide a version suffix in the cicd.json file");

            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));

            var request = CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings;
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

            Logger.Success($"updated prerelease version {nextReleaseNumber}-{versionSuffix} in csproj file {projectPath}");
        });

    private Target GitPushReleaseFiles => _ => _
        .DependsOn(SetupGitInPipeline)
        .Executes(() =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

            Git($"add {CICDFile.ReleaseSettings.CurrentVersionFilePath}", logOutput: true);

            if (CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings is not null)
            {
                var projectPath = CICDFile.ReleaseSettings.UpdateVersionInCSProjSettings.ProjectPath;
                Git($"add {projectPath}", logOutput: true);
            }

            if (CICDFile.ReleaseSettings.GitPushReleaseFilesSettings is not null)
            {
                var foldersPath = CICDFile.ReleaseSettings.GitPushReleaseFilesSettings.FoldersPath;
                foreach (var folderPath in foldersPath)
                    Git($"add {folderPath}/**", logOutput: true);

                var filesPath = CICDFile.ReleaseSettings.GitPushReleaseFilesSettings.FilesPath;
                foreach (var filePath in filesPath)
                    Git($"add {filePath}", logOutput: true);
            }

            Git($"commit -m \"{CI_COMMIT_PREFIX} pushed files for new release\"", logOutput: true);
            Git($"push", logOutput: true);
        });

    private Target CreateNewRelease => _ => _
        .DependsOn(ListCommitsInThisPR)
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
            var releaseBodyBuilder = _serviceProvider.GetRequiredService<IReleaseNotesBuilder>();
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var releaseNumberParser = _serviceProvider.GetRequiredService<IReleaseNumberParser>();

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);
            var releaseOperatorClient = ghClient.Repository.Release;

            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));
            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");

            var nextVersion = ReadVersionFromFile(releaseNumberParser).ToString();
            Logger.Info($"Creating Release {nextVersion}");

            var newReleaseData = new NewRelease(nextVersion)
            {
                Name = nextVersion,
                Body = releaseBodyBuilder.Build(commitMsgs.ToArray())
            };

            var releaseCreated = await releaseOperatorClient.Create(repoOwner, repoName, newReleaseData);
            Logger.Success($"Release {nextVersion} created!");

            var settings = CICDFile.ReleaseSettings;
            if (settings.ReleaseAssets is not null && settings.ReleaseAssets.Any())
            {
                foreach (var releaseAsset in settings.ReleaseAssets)
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

    private ReleaseNumber ReadVersionFromFile(IReleaseNumberParser releaseNumberParser)
    {
        var jsonFileContent = File.ReadAllText(CICDFile.ReleaseSettings.CurrentVersionFilePath);
        var currentVersionFileModel = JsonSerializer.Deserialize<CurrentVersionFileModel>(jsonFileContent, SerializerContants.DESERIALIZER_OPTIONS);
        return releaseNumberParser.Parse(currentVersionFileModel.CurrentVersion);
    }
}