using cangulo.cicd.abstractions.Models.Enums;
using cangulo.cicd.domain.Parsers;
using cangulo.common.testing.dataatributes;
using FluentAssertions;
using System;
using Xunit;

namespace cangulo.cicd.domain.UT.Parser
{
    public class CommitParserShould
    {

        [Theory]
        [InlineAutoNSubstituteData("feat:bla bla", "bla bla", new string[] { "feat" }, "feat")]
        [InlineAutoNSubstituteData("feat:bla bla", "bla bla", new string[] { "Feat" }, "feat")]
        [InlineAutoNSubstituteData("Fix:bla bla", "bla bla", new string[] { "fix" }, "fix")]
        public void HappyPath_Parse_ConventionalCommit(
            string commitMsg,
            string expectedBody,
            string[] conventionalCommitTypes,
            string expectedCommitType,
            CommitParser sut)
        {
            // Arrange
            // Act
            var result = sut.ParseConventionalCommit(commitMsg, conventionalCommitTypes);

            // Assert
            result.Body.Should().Be(expectedBody);
            result.CommitType.Should().Be(expectedCommitType);
        }

        [Theory]
        [InlineAutoNSubstituteData("", new string[] { "feat" })]
        [InlineAutoNSubstituteData("bla", new string[] { "feat" })]
        [InlineAutoNSubstituteData(":", new string[] { "feat" })]
        [InlineAutoNSubstituteData("bla:", new string[] { "feat" })]
        [InlineAutoNSubstituteData(":bla", new string[] { "feat" })]
        public void ThrowException_When_InvalidInput(
            string commitMsg,
            string[] conventionalCommitTypes,
            CommitParser sut)
        {
            // Arrange
            // Act
            Action action = () => sut.ParseConventionalCommit(commitMsg, conventionalCommitTypes);

            // Assert
            action.Should().Throw<Exception>();
        }

        [Theory]
        [InlineAutoNSubstituteData("feat:bla bla", new string[] { })]
        public void ThrowException_When_NoCommitTypesProvided(
            string commitMsg,
            string[] conventionalCommitTypes,
            CommitParser sut)
        {
            // Arrange
            // Act
            Action action = () => sut.ParseConventionalCommit(commitMsg, conventionalCommitTypes);

            // Assert
            action.Should().Throw<Exception>();
        }

        [Theory]
        [InlineAutoNSubstituteData("feat:bla bla", new string[] { "fix" })]
        public void ThrowException_When_NoCommitTypeMatch(
            string commitMsg,
            string[] conventionalCommitTypes,
            CommitParser sut)
        {
            // Arrange
            // Act
            Action action = () => sut.ParseConventionalCommit(commitMsg, conventionalCommitTypes);

            // Assert
            action.Should().Throw<Exception>();
        }
    }
}
