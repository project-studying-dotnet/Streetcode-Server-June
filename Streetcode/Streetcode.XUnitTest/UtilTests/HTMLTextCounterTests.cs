using Streetcode.BLL.Util;
using Xunit;

namespace Streetcode.XUnitTest.UtilTests
{
    public class HTMLTextCounterTests
    {
        [Fact]
        public async Task Counter_Should_ReturnZero_WhenItTakeNull()
        {
            // Arrange
            int expexted = 0;
            string text = null!;

            Testing(text, expexted);
        }

        [Fact]
        public async Task Counter_Should_ReturnZero_WhenItTakeEmptyString()
        {
            // Arrange
            int expexted = 0;
            string text = string.Empty;

            Testing(text, expexted);
        }

        [Fact]
        public async Task Counter_Should_ReturnTextLength_WhenItTakeTextWithoutTags()
        {
            // Arrange
            string text = "simpl text";
            int expexted = text.Length;

            Testing(text, expexted);
        }

        [Fact]
        public async Task Counter_Should_ReturnTextLength_WhenItTakeTextWithTags()
        {
            // Arrange
            string text = "<p>Цей текст містить теги: <strong>жирний шрифт</strong>, <i>курсив</i>, <u>підкреслення</u>.</p>";
            int expexted = "Цей текст містить теги: жирний шрифт, курсив, підкреслення.".Length;

            Testing(text, expexted);
        }

        public static void Testing(string text, int expexted)
        {
            // Act
            int result = text.OnlyTextCount();

            // Assert
            Assert.Equal(expexted, result);
        }
    }
}
