using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

internal partial class Build : NukeBuild
{
    private Target Restore => _ => _
        .DependsOn(ParseCICDFile, SetTargetSolution)
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
}