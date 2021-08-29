using Nuke.Common;
using System.Linq;

internal partial class Build : NukeBuild
{
    private Target SetupGitInPipeline => _ => _
        .Executes(() =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.GitSettings, nameof(CICDFile.GitSettings));

            var request = CICDFile.GitSettings;
            Logger.Info("Setting email and name in git");

            Git($"config --global user.email \"{request.Email}\"");
            Git($"config --global user.name \"{request.Name}\"");
        });

    private Target GitPushReleaseFiles => _ => _
        .DependsOn(SetupGitInPipeline)
        .Executes(() =>
        {
            Git($"add cicd.json", logOutput: true);

            if (CICDFile.ChangelogSettings is not null)
                Git($"add CHANGELOG.md", logOutput: true);

            if (CICDFile.GitSettings.GitPushReleaseFilesSettings is not null)
            {
                var filesToPush = CICDFile.GitSettings.GitPushReleaseFilesSettings.FilesPath;

                foreach (var file in filesToPush)
                    Git($"add {file}", logOutput: true);
            }

            var releasefile = CICDFile.GitSettings.GitPushReleaseFilesSettings.FilesPath;


            Git($"commit -m \"[ci] new version {CICDFile.Versioning.CurrentVersion} created\"", logOutput: true);
            Git($"push", logOutput: false);
        });

    //private Target GetLastConventionalCommit => _ => _
    //    .Executes(() =>
    //    {
    //        var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();

    //        var lastCommitMsg = Git($"log --format=%B -n 1", logOutput: true).ConcatenateOutputText();
    //        var conventionalCommit = commitParser.ParseConventionalCommit(lastCommitMsg);

    //        Logger.Info($"ConventionCommit:\n{JsonSerializer.Serialize(conventionalCommit, SerializerContants.SERIALIZER_OPTIONS)}");
    //    });
}