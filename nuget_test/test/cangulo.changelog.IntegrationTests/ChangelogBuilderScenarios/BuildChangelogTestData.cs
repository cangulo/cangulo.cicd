using cangulo.changelog.IntegrationTests.Models;

namespace cangulo.changelog.IntegrationTests.ChangelogBuilderScenarios
{
    public class BuildChangelogTestData : TestDataBaseModel
    {
        public Input Input { get; set; }
        public string[] ExpectedOutput { get; set; }
    }
    public class Input
    {
        public string Version { get; set; }
        public string[] NewChanges { get; set; }
        public string[] PreviousChangelogFile { get; set; }
    }
}