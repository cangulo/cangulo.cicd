using AutoFixture.Xunit2;
using cangulo.changelog.Builders.ConventionalCommits;
using cangulo.changelog.Models;
using cangulo.changelog.Parsers;
using cangulo.changelog.UT.Helpers;
using cangulo.changelog.UT.Models;
using cangulo.common.testing.dataatributes;
using FluentAssertions;
using NSubstitute;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cangulo.changelog.UT.Builders
{
    public class ChangesAreaBuilderForConventionalCommitsShould
    {
        private readonly ChangesAreaBuilderForConventionalCommits sut;
        private const string TestDataPath = "./Builders/ChangesAreaBuilderTestData.json";

        public ChangesAreaBuilderForConventionalCommitsShould()
        {
            var changelogSettings = new ChangelogSettings
            {
                CommitsMode = CommitsMode.ConventionalCommits,
                ConventionalCommitsSettings = new ConventionalCommitsSettings
                {
                    Types = new string[] { "feat", "fix" }
                }
            };

            var conventionalCommitParser = new ConventionalCommitParser(changelogSettings); ;
            sut = new ChangesAreaBuilderForConventionalCommits(conventionalCommitParser, changelogSettings);
        }

        [Theory]
        [InlineAutoNSubstituteData("conventional_commits_provided")]
        public async Task HappyPath(
            string scenario)
        {

            // Arrange
            var testData = await TestDataHelper.GetTestDataForScenario<ChangesAreaBuilderTestData>(scenario, TestDataPath);
            var input = testData.Input;
            var expectedOutput = string.Join("\r\n", testData.ExpectedOutput.ToArray());

            // Act
            var result = sut.Build(input);

            // Assert
            result.Should().BeEquivalentTo(expectedOutput);
        }

        [Theory]
        [AutoNSubstituteData]
        public void ReturnEmpty_WhenInvalidEmptyChangesProvided(
            string version,
            ChangesAreaBuilderForConventionalCommits sut)
        {
            // Arrange
            var changes = new string[] { };
            // Act
            var result = sut.Build(changes);

            // Assert
            result.Should().BeEquivalentTo(string.Empty);
        }
    }

}
