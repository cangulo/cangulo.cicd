﻿using System.Linq;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;

internal partial class Build : NukeBuild
{
    private Target SetupGitInPipeline => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.GitPipelineSettings, nameof(CICDFile.GitPipelineSettings));

            var request = CICDFile.GitPipelineSettings;
            Logger.Info("Setting email and name in git");

            Git($"config --global user.email \"{request.Email}\"");
            Git($"config --global user.name \"{request.Name}\"");
        });

    private Target GitPush => _ => _
        .DependsOn(ParseCICDFile, SetupGitInPipeline)
        .Executes(() =>
        {
            Git($"add cicd.json", logOutput: true);
            Git($"commit -m \"[ci] new version {CICDFile.VersioningSettings.CurrentVersion} created\"", logOutput: true);
            Git($"push", logOutput: false);
        });

    private Target GetLastCommitMsgTarget => _ => _
        .Executes(() =>
        {
            var cmdOutput = Git($"log --no-merges --format=%B -n 1", logOutput: true);
            var commitMsg = string.Join(string.Empty, cmdOutput.Select(x => x.Text).ToArray());
            Logger.Info($"LastCommitMessage:\n{commitMsg}");
            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();
            var conventionalCommit = commitParser.ParseConventionCommitFromMergeCommit(commitMsg);
        });
}