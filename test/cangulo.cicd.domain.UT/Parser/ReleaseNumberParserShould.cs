using cangulo.cicd.domain.Parsers;
using cangulo.common.testing.dataatributes;
using FluentAssertions;
using System;
using Xunit;

namespace cangulo.cicd.domain.UT.Parser
{
    public class ReleaseNumberParserShould
    {
        [Theory]
        [InlineAutoNSubstituteData("1.1.1", 1, 1, 1)]
        public void Parse_ReleaseNumber(
            string commitMsg,
            int major,
            int minor,
            int patch,
            ReleaseNumberParser sut)
        {
            // Arrange
            // Act
            var result = sut.Parse(commitMsg);

            // Assert
            result.Major.Should().Be(major);
            result.Minor.Should().Be(minor);
            result.Patch.Should().Be(patch);
        }

        [Theory]
        [InlineAutoNSubstituteData("")]
        [InlineAutoNSubstituteData(".")]
        [InlineAutoNSubstituteData(".")]
        [InlineAutoNSubstituteData("asd.asda.asd")]
        [InlineAutoNSubstituteData("1.asda.asd")]
        [InlineAutoNSubstituteData("1.2.asd")]
        public void ThrowException_For_EmptyNumbers(
            string commitMsg,
            ReleaseNumberParser sut)
        {
            // Arrange
            // Act
            Action action = () => sut.Parse(commitMsg);

            // Assert
            action.Should().Throw<Exception>();
        }
    }
}
