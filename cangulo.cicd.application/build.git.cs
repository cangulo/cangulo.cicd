using Nuke.Common;
using System.Linq;

internal partial class Build : NukeBuild
{
    private Target SetupGitInPipeline => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.GitPipelineSettings, nameof(CICDFile.GitPipelineSettings));

            // Git($"config --global user.email \"carlos.angulo.mascarell@outlook.com\"", logOutput: true);
            // Git($"config --global user.name \"Carlos Angulo\"", logOutput: true);
        });

    private Target GitPush => _ => _
        .DependsOn(ParseCICDFile, SetupGitInPipeline)
        .Executes(() =>
        {
            Git($"add cicd.json", logOutput: true);
            Git($"commit -am \"[ci] new version {CICDFile.VersioningSettings.CurrentVersion} created\"", logOutput: true);
            var outputCmd = Git($"push --force", logOutput: false);

            outputCmd.ToList().ForEach(x => Logger.Info($"{x.Type} - {x.Text}"));

        });
}