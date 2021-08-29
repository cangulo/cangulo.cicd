using cangulo.changelog.Builders.NonConventionalCommits;
using cangulo.changelog.UT.Helpers;
using cangulo.changelog.UT.Models;
using cangulo.common.testing.dataatributes;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cangulo.changelog.UT.Builders
{
    public class ChangesAreaBuilderForNonConventionalCommitsShould
    {
        private const string TestDataPath = "./Builders/ChangesAreaBuilderTestData.json";

        [Theory]
        [InlineAutoNSubstituteData("non_conventional_commits_provided")]
        public async Task HappyPath(string scenario, ChangesAreaBuilderForNonConventionalCommits sut)
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
        public void ReturnEmpty_WhenNoChangesProvided(
            ChangesAreaBuilderForNonConventionalCommits sut)
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
