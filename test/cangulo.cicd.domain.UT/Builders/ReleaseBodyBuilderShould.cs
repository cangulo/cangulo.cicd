﻿using cangulo.cicd.domain.Builders;
using cangulo.cicd.UT.common;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace cangulo.cicd.domain.UT.Builders
{
    public class ReleaseBodyBuilderShould
    {
        public class Input
        {
            public string[] Changes { get; set; }
        }

        [Theory]
        [InlineAutoNSubstituteData(
            ".\\Builders\\testData\\ReleaseBodyBuider_happyPath_input.json",
            ".\\Builders\\testData\\ReleaseBodyBuider_happyPath_expected.txt"
            )]
        public async Task Build_HappyPath(
            string inputTestDataJsonFilePath,
            string expectedResultFilePath,
            ReleaseBodyBuilder sut)
        {
            // Arrange
            var input = await JsonTestDataParser.DeserializeFile<Input>(inputTestDataJsonFilePath);
            var expectedResult = await TextFileReader.ReadAsync(expectedResultFilePath);

            // Act
            var result = sut.Build(input.Changes);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }


        [Theory]
        [AutoNSubstituteData]
        public void ReturnEmpty_WhenInvalidEmptyChangesProvided(string version, ReleaseBodyBuilder sut)
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