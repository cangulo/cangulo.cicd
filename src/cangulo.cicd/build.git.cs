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
}