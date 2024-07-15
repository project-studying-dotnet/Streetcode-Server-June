using Streetcode.BLL.Util.Comments;
using Xunit;

namespace Streetcode.XUnitTest.UtilTests
{
    public class BadWordsVerificationTests
    {
        [Fact]
        public async Task Verification_Should_ReturnTrue_WhenItTakeStringWithoutBadWords()
        {
            // Arrange
            string text = "hello nice to meet you";
            bool expexted = true;

            // Act
            bool result = BadWordsVerification.NotContainBadWords(text);

            // Assert
            Assert.Equal(expexted, result);
        }

        [Fact]
        public async Task Verification_Should_ReturnFalse_WhenItTakeStringWithBadWords()
        {
            // Arrange
            string text = "You son of a bitch! I'm going to kill you!";
            bool expexted = false;

            // Act
            bool result = BadWordsVerification.NotContainBadWords(text);

            // Assert
            Assert.Equal(expexted, result);
        }
    }
}
