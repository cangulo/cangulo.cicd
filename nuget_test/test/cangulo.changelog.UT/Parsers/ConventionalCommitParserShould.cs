using AutoFixture.Xunit2;
using cangulo.changelog.Models;
using cangulo.changelog.Parsers;
using cangulo.changelog.UT.Helpers;
using cangulo.changelog.UT.Models;
using cangulo.common.testing;
using cangulo.common.testing.dataatributes;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cangulo.changelog.UT.Parsers
{
    public class ConventionalCommitParserShould
    {
        private const string TestDataPath = "./Parsers/ConventionalCommitParserTestData.json";

        private static void SetChangeLogSettings(ChangelogSettings changelogSettings)
        {
            changelogSettings.CommitsMode = CommitsMode.ConventionalCommits;
            changelogSettings.ConventionalCommitsSettings = new ConventionalCommitsSettings
            {
                Types = new string[] { "feat", "fix" }
            };
        }

        [Theory]
        [InlineAutoNSubstituteData("conventional_type_feat")]
        [InlineAutoNSubstituteData("conventional_type_fix")]
        [InlineAutoNSubstituteData("invalid_conventional_type_provided")]
        [InlineAutoNSubstituteData("no_conventional_type_provided")]
        public async Task Should_Process_ConventionalCommits(
            string scenario,
            [Frozen] ChangelogSettings changelogSettings,
            ConventionalCommitParser sut)
        {
            // Arrange
            SetChangeLogSettings(changelogSettings);
            var testData = await TestDataHelper.GetTestDataForScenario<ConventionalCommitParserTestData>(scenario, TestDataPath);
            var input = testData.Input;
            var output = testData.Output;

            // Act
            var result = sut.Parse(input);

            // Assert
            result.Should().BeEquivalentTo(output);
        }
    }
}