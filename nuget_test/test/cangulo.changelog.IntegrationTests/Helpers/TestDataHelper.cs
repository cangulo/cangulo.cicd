using cangulo.changelog.IntegrationTests.Models;
using cangulo.common.testing;
using System.Linq;
using System.Threading.Tasks;

namespace cangulo.changelog.IntegrationTests.Helpers
{
    public static class TestDataHelper
    {
        public static async Task<T> GetTestDataForScenario<T>(string scenario, string filePath) where T : TestDataBaseModel
        {
            var testCases = await JsonTestDataParser.DeserializeFile<T[]>(filePath);
            return testCases.Single(x => x.Scenario == scenario);
        }
    }
}