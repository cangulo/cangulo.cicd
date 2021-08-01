using cangulo.cicd.Abstractions.Constants;
using static Nuke.Common.IO.FileSystemTasks;
using cangulo.cicd.domain.Extensions;
using cangulo.cicd.domain.Helpers;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Octokit;
using System.IO;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild

{
    private Target CalculateNextReleaseNumber => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.VersioningSettings, "versioningSettings should be provided in the cicd.json");
            var request = CICDFile.VersioningSettings;

            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = _serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = _serviceProvider.GetRequiredService<IReleaseNumberParser>();

            var currentReleaseNumber = releaseNumberParser.Parse(request.CurrentVersion);

            var commitMsg = GetLastCommitMsg();
            var conventionalCommit = commitParser.ParseConventionCommit(commitMsg);

            var releaseType = conventionalCommit.CommitType.ToReleaseType();
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            Logger.Info($"next release Number:{nextReleaseNumber} - Release Type: {releaseType}");
            CICDFile.VersioningSettings.CurrentVersion = nextReleaseNumber.ToString();
        });

    private Target UpdateVersionInFiles => _ => _
        .DependsOn(ParseCICDFile, CalculateNextReleaseNumber)
        .Executes(() =>
        {
            var cicdFilePath = RootDirectory / "cicd.json";

            Logger.Info($"{JsonSerializer.Serialize(GitHubActions, SerializerContants.SERIALIZER_OPTIONS)}");

            var content = JsonSerializer.Serialize(CICDFile, SerializerContants.SERIALIZER_OPTIONS);
            File.WriteAllText(cicdFilePath, content);

            Git($"config --global user.email \"carlos.angulo.mascarell@outlook.com\"", logOutput: true);
            Git($"config --global user.name \"Carlos Angulo\"", logOutput: true);

            Git($"remote", logOutput: true);
            Git($"remote set-url origin https://x-access-token:{GitHubToken}@github.com/{GitHubActions.GitHubRepository}", logOutput: true);
            //Git("checkout \"${ GITHUB_REF: 11}\"", logOutput: true);

            Git($"status", logOutput: true);
            Git($"add cicd.json", logOutput: true);
            Git($"status", logOutput: true);
            Git($"commit -m \"[ci] new version {CICDFile.VersioningSettings.CurrentVersion} created\"", logOutput: true);
            Git($"push origin {GitHubActions.GitHubRef}", logOutput: true);
        });

    private Target ZipReleaseAssetDirectory => _ => _
        .DependsOn(ParseCICDFile, Publish)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.VersioningSettings, "versioningSettings should be provided in the cicd.json");

            Logger.Info($"Zipping the release asset directroy");

            var releaseAssetDirectory = (RootDirectory / CICDFile.VersioningSettings.ReleaseAssetDirectory);
            var assetDirectoryZipped = RootDirectory / $"{CICDFile.VersioningSettings.ReleaseAssetName}.zip";
            CompressionTasks.CompressZip(releaseAssetDirectory, assetDirectoryZipped);
        });
    private Target CreateNewRelease => _ => _
        .DependsOn(ParseCICDFile, UpdateVersionInFiles, ZipReleaseAssetDirectory)
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);

            // TODO: Migrate the injection of the client to an interface
            var ghClient = new GitHubClient(new ProductHeaderValue($"{repoOwner}"));
            ghClient.Credentials = new Credentials(GitHubToken);
            var client = ghClient.Repository.Release;

            Logger.Info($"{repoName} - {repoOwner}");

            var newReleaseData = new NewRelease(CICDFile.VersioningSettings.CurrentVersion.ToString())
            {
                // TODO: Define a better body
                Name = "empty body... for now"
            };

            var releaseCreated = await client.Create(repoOwner, repoName, newReleaseData);

            var releaseCreatedInfo = new
            {
                releaseCreated.Id,
                releaseCreated.Name,
                releaseCreated.TagName,
                releaseCreated.TargetCommitish,
                releaseCreated.AssetsUrl
            };
            Logger.Success($"Created release: {JsonSerializer.Serialize(releaseCreatedInfo)}");

            var assetDirectoryZipped = RootDirectory / $"{CICDFile.VersioningSettings.ReleaseAssetName}.zip";
            var fileName = Path.GetFileName(assetDirectoryZipped);
            Logger.Info($"Uploading file: {fileName}");

            var assetData = new ReleaseAssetUpload
            {
                FileName = fileName,
                RawData = File.OpenRead(assetDirectoryZipped),
                ContentType = "application/zip"
            };
            await client.UploadAsset(releaseCreated, assetData);
        });

    private string GetLastCommitMsg()
    {
        var lastCommitId = Git($"log --no-merges --format=%H -n 1", logInvocation: false, logOutput: false).Select(x => x.Text).Single();

        Logger.Info($"Last Commit: {lastCommitId} Message:");
        var cmdOutput = Git($"show {lastCommitId} -q --oneline --format=%B", logInvocation: true, logOutput: true);

        ControlFlow.Assert(cmdOutput.All(x => x.Type == OutputType.Std), "error getting last commit");

        var commitMsg = string.Join('\n', cmdOutput.Select(x => x.Text).ToArray());
        ControlFlow.Assert(!string.IsNullOrEmpty(commitMsg), "last commit does not have a msg");
        return commitMsg;
    }
}