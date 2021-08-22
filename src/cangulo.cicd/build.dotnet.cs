using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using System.IO;
using Nuke.Common.ProjectModel;
using System;

internal partial class Build : NukeBuild
{
    private Target SetTargetSolution => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
            {
                var solutionPath = RootDirectory / CICDFile.DotnetTargets.SolutionPath;
                if (File.Exists(solutionPath))
                    TargetSolutionParsed = ProjectModelTasks.ParseSolution(solutionPath);
                else
                    throw new Exception("invalid SolutionPath provided");
                Logger.Info($"solution set to {TargetSolutionParsed.Name}");
            });

    private Target CleanBuildFolders => _ => _
        .Before(Restore)
        .Executes(() =>
            {
                RootDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            });
    private Target Restore => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution, CleanBuildFolders)
        .Executes(() =>
         {
             DotNetRestore(s => s
                     .SetProjectFile(TargetSolutionParsed));
         });

    private Target Compile => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution, Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(TargetSolutionParsed)
                .EnableNoRestore());
        });

    private Target ExecuteUnitTests => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution, Compile)
        .Executes(() =>
        {
            Logger.Info($"Executing UT");

            DotNetTest(s => s
                .SetProjectFile(TargetSolutionParsed)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    private Target Publish => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution)
        .Before(CompressDirectory)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.DotnetTargets.DotnetPublish, "DotnetPublish should be provided in the cicd.json");
            var inputSettings = CICDFile.DotnetTargets.DotnetPublish;

            var projectPath = RootDirectory / inputSettings.ProjectPath;
            if (FileExists(projectPath))
            {
                var outputDirectory = RootDirectory / inputSettings.OutputFolder;
                EnsureCleanDirectory(outputDirectory);

                var cmdSettings = new DotNetPublishSettings()
                                    .SetProject(projectPath)
                                    .SetOutput(outputDirectory);

                if (!string.IsNullOrEmpty(inputSettings.RunTime))
                    cmdSettings = cmdSettings
                    .SetRuntime(inputSettings.RunTime)
                    .SetSelfContained(true);

                DotNetPublish(cmdSettings);
            }
        });
}