using cangulo.cicd;
using Nuke.Common;
using System;

internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.GetLastConventionCommit);

    private readonly IServiceProvider _serviceProvider;

    public Build()
    {
        _serviceProvider = Startup.RegisterServices();
    }
}