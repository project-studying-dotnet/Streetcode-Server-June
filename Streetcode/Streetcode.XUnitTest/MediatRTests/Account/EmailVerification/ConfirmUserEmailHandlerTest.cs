using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail;
using Streetcode.BLL.Resources;
using Xunit;
using static System.Formats.Asn1.AsnWriter;

namespace Streetcode.XUnitTest.MediatRTests.Account.EmailVerification
{
    public class ConfirmUserEmailHandlerTest
    {
        private readonly UserManager<IdentityUser> _mockUserManager;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly ConfirmUserEmailHandler _handler;
        private readonly Mock<IUserStore<IdentityUser>> _userStore;
        private ConfirmUserEmailCommand _command = new ConfirmUserEmailCommand(null!, null!);

        public ConfirmUserEmailHandlerTest()
        {
            _mockLogger = new Mock<ILoggerService>();

            _userStore = new Mock<IUserStore<IdentityUser>>();

            _mockUserManager = new UserManager<IdentityUser>(_userStore.Object, null, null, null, null, null, null, null, null);

            _handler = new ConfirmUserEmailHandler(_mockUserManager, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenRequestIdIsNull()
        {
            var result = await Act(null!, "token");
            string expected = "userId or token is empty";

            ErrorAssert(result, expected);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenRequestTokenIsNull()
        {
            var result = await Act("Id", null!);
            string expected = "userId or token is empty";

            ErrorAssert(result, expected);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenRequestIdAndTokenAreNull()
        {
            var result = await Act(null!, null!);
            string expected = "userId or token is empty";

            ErrorAssert(result, expected);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenUserIsNull()
        {
            _userStore.Setup(x => x.FindByIdAsync("Id", CancellationToken.None))
               .ReturnsAsync((IdentityUser)null!);

            var result = await Act("Id", "token");
            string expected = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, _command);

            ErrorAssert(result, expected);
        }

        private async Task<Result<string>> Act(string id, string token)
        {
            _command = new ConfirmUserEmailCommand(id, token);

            var result = await _handler
                .Handle(_command, CancellationToken.None);

            return result;
        }

        private void ErrorAssert(Result<string> result, string expected)
        {
            Assert.Multiple(
                () => Assert.True(result.IsFailed),
                () => Assert.Equal(expected, result.Errors.FirstOrDefault()?.Message));
        }
    }
}
