using cangulo.changelog.Models;

namespace cangulo.changelog.UT.Models
{
    public class ConventionalCommitParserTestData : TestDataBaseModel
    {
        public string Input { get; set; }
        public ConventionalCommit Output { get; set; }
    }
}
