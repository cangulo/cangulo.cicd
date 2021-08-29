using cangulo.changelog.UT.Models;
using cangulo.common.testing;
using System.Linq;
using System.Threading.Tasks;

namespace cangulo.changelog.UT.Helpers
{
    public static class TestDataHelper
    {
        public static async Task<T> GetTestDataForScenario<T>(string scenario, string path) where T : TestDataBaseModel
        {
            var testCases = await JsonTestDataParser.DeserializeFile<T[]>(path);
            return testCases.Single(x => x.Scenario == scenario);
        }
    }
}