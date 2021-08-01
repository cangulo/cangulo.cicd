using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using System;
using System.IO;

internal partial class Build : NukeBuild
{
    private Target CompressDirectory => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.CompressDirectory, "CompressDirectory should be provided in the cicd.json");

            var request = CICDFile.CompressDirectory;

            var path = (RootDirectory / request.Path);
            var outputFileName = RootDirectory / request.OutputFileName;

            ControlFlow.Assert(outputFileName.ToString().ToLowerInvariant().Contains(".zip"),"OutputFileName should end with .zip");
            
            Logger.Info($"zipping directory: {request.Path} into file: {outputFileName}");

            CompressionTasks.CompressZip(path, outputFileName);
        });

}