using Streetcode.BLL.Util;
using Xunit;

namespace Streetcode.XUnitTest.UtilTests
{
    public class DateToStringConverterTests
    {
        [Fact]
        public async Task Converter_Should_ReturnDoubleDateString_WhenItTakeTwoDate()
        {
            // Arrange
            string expexted = "20 січня 2008 - 21 березня 2012";
            DateTime first = new DateTime(2008, 1, 20);
            DateTime second = new DateTime(2012, 3, 21);

            // Act
            string result = DateToStringConverter.CreateDateString(first, second);

            // Assert
            Assert.Equal(expexted, result);
        }

        [Fact]
        public async Task Converter_Should_ReturnSimplDateString_WhenItTakeOneDate()
        {
            // Arrange
            string expexted = "20 січня 2008";
            DateTime first = new DateTime(2008, 1, 20);
            DateTime? second = null;

            // Act
            string result = DateToStringConverter.CreateDateString(first, second);

            // Assert
            Assert.Equal(expexted, result);
        }
    }
}
