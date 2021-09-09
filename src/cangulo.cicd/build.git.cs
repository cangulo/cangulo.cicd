using cangulo.cicd.abstractions.Constants;
using Nuke.Common;

internal partial class Build : NukeBuild
{
    private const string CI_COMMIT_PREFIX = CiActionsContants.CI_ACTION_COMMIT_PREFIX;

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

            if (CICDFile.Versioning.UpdateVersionInCSProjSettings is not null)
            {
                var projectPath = CICDFile.Versioning.UpdateVersionInCSProjSettings.ProjectPath;
                Git($"add {projectPath}", logOutput: true);
            }
            Git($"commit -m \"{CI_COMMIT_PREFIX} new version {CICDFile.Versioning.CurrentVersion} created\"", logOutput: true);
            Git($"push", logOutput: false);
        });
}