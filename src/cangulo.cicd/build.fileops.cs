using Nuke.Common;
using Nuke.Common.IO;

internal partial class Build : NukeBuild
{
    private Target CompressDirectory => _ => _
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.CompressDirectory, "CompressDirectory should be provided in the cicd.json");

            var request = CICDFile.CompressDirectory;

            var path = (RootDirectory / request.Path);
            var outputFileName = RootDirectory / request.OutputFileName;

            ControlFlow.Assert(outputFileName.ToString().ToLowerInvariant().Contains(".zip"), "OutputFileName should end with .zip");

            Logger.Info($"zipping directory: {request.Path} into file: {outputFileName}");

            // CompressionTasks.Compress(path, outputFileName);
            // zip -j -r ./releaseAssets/cangulo.cicd-linux-x64.zip ./artifacts/cangulo.cicd/
            FileSystemTasks.EnsureExistingParentDirectory(outputFileName);
            Zip($"-j -r {outputFileName} {path}/");
        });

}