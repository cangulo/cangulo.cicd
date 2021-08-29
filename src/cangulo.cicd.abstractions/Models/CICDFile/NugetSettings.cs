namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class NugetSettings
    {

        public PackNugetSettings PackNugetSettings { get; set; }
        public PushNugetsSettings PushNugetsSettings { get; set; }

    }

    public class PackNugetSettings
    {
        public string ProjectPath { get; set; }
        public string OutputDirectory { get; set; }
    }

    public class PushNugetsSettings
    {
        public string NugetsDirectory { get; set; }
        public string Source { get; set; }
        public bool ApiKeyRequired { get; set; }
    }
}
