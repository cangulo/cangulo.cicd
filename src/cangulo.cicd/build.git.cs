using Nuke.Common;

internal partial class Build : NukeBuild
{
    private Target SetupGitInPipeline => _ => _
        .Executes(() =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.GitTargets, nameof(CICDFile.GitTargets));

            var request = CICDFile.GitTargets;
            Logger.Info("Setting email and name in git");

            Git($"config --global user.email \"{request.Email}\"");
            Git($"config --global user.name \"{request.Name}\"");
        });

    private Target GitPush => _ => _
        .DependsOn(SetupGitInPipeline)
        .Executes(() =>
        {
            // TODO: add all changes and push

            Git($"add cicd.json", logOutput: true);
            Git($"commit -m \"[ci] new version {CICDFile.Versioning.CurrentVersion} created\"", logOutput: true);
            Git($"push", logOutput: false);
        });

    //private Target GetLastConventionalCommit => _ => _
    //    .Executes(() =>
    //    {
    //        var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();

    //        var lastCommitMsg = Git($"log --format=%B -n 1", logOutput: true).ConcatenateOutputText();
    //        var conventionalCommit = commitParser.ParseConventionalCommit(lastCommitMsg);

    //        Logger.Info($"ConventionCommit:\n{JsonSerializer.Serialize(conventionalCommit, SerializerContants.SERIALIZER_OPTIONS)}");
    //    });
}