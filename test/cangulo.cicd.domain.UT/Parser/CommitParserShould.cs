using cangulo.cicd.abstractons.Models.Enums;
using cangulo.cicd.domain.Parsers;
using cangulo.cicd.UT.common;
using FluentAssertions;
using System;
using Xunit;

namespace cangulo.cicd.domain.UT.Parser
{
    public class CommitParserShould
    {

        [Theory]
        [InlineAutoNSubstituteData("feat:bla bla", "bla bla", CommitType.Feat)]
        [InlineAutoNSubstituteData("fix:bla bla", "bla bla", CommitType.Fix)]
        [InlineAutoNSubstituteData("patch:bla bla", "bla bla", CommitType.Patch)]
        [InlineAutoNSubstituteData("major:bla bla", "bla bla", CommitType.Major)]
        public void Parse_ConventionCommit(
            string commitMsg,
            string expectedBody,
            CommitType expectedCommitType,
            CommitParser sut)
        {
            // Arrange
            // Act
            var result = sut.ParseConventionalCommit(commitMsg);

            // Assert
            result.Body.Should().Be(expectedBody);
            result.CommitType.Should().Be(expectedCommitType);
        }

        [Theory]
        [InlineAutoNSubstituteData("")]
        [InlineAutoNSubstituteData("bla")]
        [InlineAutoNSubstituteData(":")]
        [InlineAutoNSubstituteData("bla:")]
        [InlineAutoNSubstituteData(":bla")]
        public void ThrowException_When_NoCommitTypeProvided(
            string commitMsg,
            CommitParser sut)
        {
            // Arrange
            // Act
            Action action = () => sut.ParseConventionalCommit(commitMsg);

            // Assert
            action.Should().Throw<Exception>();
        }
    }
}
