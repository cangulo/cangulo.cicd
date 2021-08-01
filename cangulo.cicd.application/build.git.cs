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
using cangulo.cicd.abstractons.Models.Enums;

internal partial class Build : NukeBuild
{
    private Target SetupGitInPipeline => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.GitPipelineSettings);

            Git($"config --global user.email \"carlos.angulo.mascarell@outlook.com\"", logOutput: true);
            Git($"config --global user.name \"Carlos Angulo\"", logOutput: true);
        });

    private Target GitPush => _ => _
        .DependsOn(ParseCICDFile, SetupGitInPipeline)
        .Executes(() =>
        {
            Git($"add cicd.json", logOutput: true);
            Git($"commit -m \"[ci] new version {CICDFile.VersioningSettings.CurrentVersion} created\"", logOutput: true);
            Git($"push", logOutput: true);
        });
}