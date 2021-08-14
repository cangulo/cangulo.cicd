using Nuke.Common;

internal partial class Build : NukeBuild
{
    private void ValidateCICDPropertyIsProvided(object obj, string propertyName)
        => ControlFlow.NotNull(obj, $"{propertyName} should be provided in the cicd.json");
}