﻿using Nuke.Common;

internal partial class Build : NukeBuild
{
    private void ValidateCICDPropertyIsProvided(object obj)
    {
        var errorMsg = $"{obj.GetType().Name} should be provided in the cicd.json";
        ControlFlow.NotNull(obj, errorMsg);
    }
    private void ValidateObjectNotNull(object obj, string errorMsg)
    {
        ControlFlow.NotNull(obj, errorMsg);
    }
}