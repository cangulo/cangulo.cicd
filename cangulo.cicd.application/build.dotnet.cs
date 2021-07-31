using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

internal partial class Build : NukeBuild
{
    private Target Restore => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution, Clean)
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
            DotNetTest(s => s
                .SetProjectFile(TargetSolutionParsed)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    private Target Publish => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution, ExecuteUnitTests)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.DotnetPublish, "DotnetPublish should be provided in the cicd.json");
            var inputSettings = CICDFile.DotnetPublish;

            var projectPath = RootDirectory / inputSettings.ProjectPath;
            if (FileExists(projectPath))
            {
                var outputDirectory = RootDirectory / inputSettings.OutputFolder;
                EnsureCleanDirectory(outputDirectory);

                var cmdSettings = new DotNetPublishSettings()
                                    .SetProject(projectPath)
                                    .SetOutput(outputDirectory);

                if (!string.IsNullOrEmpty(inputSettings.RunTime))
                    cmdSettings = cmdSettings.SetRuntime(inputSettings.RunTime);

                DotNetPublish(cmdSettings);
            }
        });
}