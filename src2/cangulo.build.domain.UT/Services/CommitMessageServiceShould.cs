using cangulo.build.abstractions.Models.Enums;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace cangulo.build.domain.UT.Services
{
    public class CommitMessageServiceShould
    {
        [Theory]
        [InlineData("[CI] create patch", CommitAction.CreatePatch)]
        [InlineData("[CI] create minor", CommitAction.CreateMinor)]
        [InlineData("[CI] create major", CommitAction.CreateMajor)]
        [InlineData("No CI Action Type provided", CommitAction.Undefined)]
        public void InjectAllValues_For_IntType(string msg, CommitAction expectedActionType)
        {
            // Arrange
            // TODO: Inject this as parameter in the method
            var sut = new CommitMessageService();

            var result2 = Regex.IsMatch(msg, @"\[CI\] create patch", RegexOptions.IgnoreCase);

            // Act
            var result = sut.GetAction(msg);

            // Assert
            result.Should().Be(expectedActionType);
        }
    }
}