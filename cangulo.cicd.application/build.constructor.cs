using cangulo.cicd;
using Nuke.Common;
using System;

internal partial class Build : NukeBuild
{
    private readonly IServiceProvider _serviceProvider;

    public Build()
    {
        _serviceProvider = Startup.RegisterServices();
    }
}