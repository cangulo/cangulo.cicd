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
        .Executes(() =>
            {
                var solutionPath = RootDirectory / CICDFile.DotnetSettings.SolutionPath;
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
        .DependsOn(SetTargetSolution, CleanBuildFolders)
        .Executes(() =>
         {
             DotNetRestore(s => s
                     .SetProjectFile(TargetSolutionParsed));
         });

    private Target Compile => _ => _
        .DependsOn(SetTargetSolution, Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(TargetSolutionParsed)
                .EnableNoRestore());
        });

    private Target ExecuteUnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(TargetSolutionParsed)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    private Target Publish => _ => _
        .DependsOn(SetTargetSolution)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.DotnetSettings.DotnetPublish, "DotnetPublish should be provided in the cicd.json");
            var inputSettings = CICDFile.DotnetSettings.DotnetPublish;

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