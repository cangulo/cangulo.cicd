using cangulo.changelog.Models;
using cangulo.changelog.builders;
using cangulo.changelog.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace cangulo.changelog.IntegrationTests.ReleaseNotesBuilderScenarios.NonConventionalCommits
{
    public partial class BuildReleaseNotes_For_ConventionalCommits
    {

        private readonly IReleaseNotesBuilder sut;
        private const string TestDataPath = "./ReleaseNotesBuilderScenarios/ConventionalCommits/BuildReleaseNotesTestData.json";

        public BuildReleaseNotes_For_ConventionalCommits()
        {
            var changelogSettings = new ChangelogSettings
            {
                CommitsMode = CommitsMode.ConventionalCommits,
                ConventionalCommitsSettings = new ConventionalCommitsSettings
                {
                    Types = new string[] { "feat", "fix" }
                }
            };

            var serviceProvider = ServicesForTestBuilder.GetServiceProvider(changelogSettings);
            sut = serviceProvider.GetRequiredService<IReleaseNotesBuilder>();
        }

        [Theory]
        [InlineData("conventional_commits_provided")]
        [InlineData("non_conventional_commits_provided")]
        [InlineData("mix_convenvional_and_not_conventional_commits")]
        public async Task HappyPath(string scenario)
        {

            // Arrange
            var testData = await TestDataHelper.GetTestDataForScenario<ReleaseNotesTestData>(scenario, TestDataPath);
            var input = testData.Input;
            var expectedOutput = string.Join("\r\n", testData.ExpectedOutput.ToArray());

            // Act
            var result = sut.Build(input);

            // Assert
            result.Should().BeEquivalentTo(expectedOutput);
        }
    }
}
