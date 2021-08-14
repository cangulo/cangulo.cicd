using cangulo.cicd.domain.Builders;
using cangulo.cicd.domain.Parsers;
using cangulo.cicd.UT.common;
using FluentAssertions;
using System;
using Xunit;

namespace cangulo.cicd.domain.UT.Builders
{
    public class ReleaseBodyBuilderShould
    {
        [Theory]
        [AutoNSubstituteData]
        public void Parse_ReleaseNumber(
            string[] changes,
            string version,
            ReleaseBodyBuilder sut)
        {
            // Arrange
            // Act
            var result = sut.Build(changes, version);

            // Assert
        }
    }
}
