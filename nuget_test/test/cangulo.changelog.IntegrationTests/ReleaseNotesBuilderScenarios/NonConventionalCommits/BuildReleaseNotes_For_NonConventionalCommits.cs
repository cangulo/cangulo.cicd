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
    public class BuildReleaseNotes_For_NonConventionalCommits
    {
        private readonly IReleaseNotesBuilder sut;
        private const string TestDataPath = "./ReleaseNotesBuilderScenarios/NonConventionalCommits/BuildReleaseNotesTestData.json";

        public BuildReleaseNotes_For_NonConventionalCommits()
        {
            var changelogSettings = new ChangelogSettings
            {
                CommitsMode = CommitsMode.NonConventionalCommits
            };

            var serviceProvider = ServicesForTestBuilder.GetServiceProvider(changelogSettings);
            sut = serviceProvider.GetRequiredService<IReleaseNotesBuilder>();
        }

        [Theory]
        [InlineData("non_conventional_commits_provided")]
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
