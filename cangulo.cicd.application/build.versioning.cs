using cangulo.cicd.Abstractions.Constants;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Nuke.Common.Tooling;
using System;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target ProposeNextVersion => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
         {
             Logger.Info($"GitRepoInfo:{JsonSerializer.Serialize(GitRepository, SerializerContants.SERIALIZER_OPTIONS)}");

             var lastCommitId = Git($"log --no-merges --format=%H -n 1", logInvocation: false, logOutput: false).Select(x => x.Text).Single();

             Logger.Info($"Last Commit: {lastCommitId} Message:");
             var cmdOutput = Git($"show {lastCommitId} -q --oneline --format=%B", logInvocation: true, logOutput: true);

             ControlFlow.Assert(cmdOutput.All(x => x.Type == OutputType.Std), "error getting last commit");

             var commitMsg = string.Join('\n', cmdOutput.Select(x => x.Text).ToArray());
             ControlFlow.Assert(!string.IsNullOrEmpty(commitMsg), "last commit does not have a msg");

             var commitParser = _serviceProvider.GetRequiredService<ICommitParser>() ?? throw new ArgumentNullException(nameof(ICommitParser));

             var conventionCommit = commitParser.ParseConventionCommit(commitMsg);
             Logger.Info($"Commit Type: {conventionCommit.CommitType} ");

             // TODO:
             // 1. output proposed version
             // 2. Set proposed version in the cicd.json file
         });
}