using cangulo.changelog.IntegrationTests.Models;

namespace cangulo.changelog.IntegrationTests.ReleaseNotesBuilderScenarios
{
    public class ReleaseNotesTestData : TestDataBaseModel
    {
        public string[] Input { get; set; }
        public string[] ExpectedOutput { get; set; }
    }
}
