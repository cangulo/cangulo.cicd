using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using System;
using System.IO;

internal partial class Build : NukeBuild
{
    private Target SetTargetSolution => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
            {
                var solutionPath = RootDirectory / CICDFile.SolutionPath;
                if (File.Exists(solutionPath))
                    TargetSolutionParsed = ProjectModelTasks.ParseSolution(solutionPath);
                else
                    throw new Exception("invalid SolutionPath provided");
                Logger.Info($"solution set to {TargetSolutionParsed.Name}");
            });

    private Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
            {
                RootDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                //EnsureCleanDirectory(ArtifactsDirectory);
            });
}