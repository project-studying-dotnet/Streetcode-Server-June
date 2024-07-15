using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.URL;
using Streetcode.BLL.MediatR.Account.RestorePassword;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.URL;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.RestorePasswordRequestTests
{
    public class RestorePasswordRequestHandlerTests
    {
        private readonly RestorePasswordRequestHandler _handler;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IURLGenerator> _urlGenMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpaccessorMock;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ResetPasswordConfiguration _resPassConfig;
        private readonly RestorePasswordRequest _request;
        private const string EMAIL = "SomeEmail";

        public RestorePasswordRequestHandlerTests()
        {            
            _loggerMock = new Mock<ILoggerService>();
            _emailServiceMock = new Mock<IEmailService>();
            _urlGenMock = new Mock<IURLGenerator>();
            _httpaccessorMock = new Mock<IHttpContextAccessor>();
            _httpaccessorMock.Setup(c => c.HttpContext).Returns(new DefaultHttpContext());
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _emailConfiguration = new EmailConfiguration();
            _resPassConfig = new ResetPasswordConfiguration();
            _request = new RestorePasswordRequest(
                new RestorePasswordRequestDto()
                { Email = EMAIL });

            _handler = new RestorePasswordRequestHandler(
                _loggerMock.Object,
                _emailServiceMock.Object,
                _urlGenMock.Object,
                _httpaccessorMock.Object,
                _userManagerMock.Object,
                _emailConfiguration,
                _resPassConfig
                );
        }

        [Fact]
        public async Task ReturnsFailure_WhenUserNotFound()
        {            
            // Arrange
            _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var r = await _handler.Handle(
                _request,
                CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.Equal(r.Errors.First().Message,
                string.Format(ErrorMessages.UserWithEmailNotFound, EMAIL)));
        }

        [Fact]
        public async Task ReturnsFailure_WhenFailToGenerateRestoreToken()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => { return new User(); });
            _userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(() => { return null; });

            // Act
            var r = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(r.IsFailed),
                () => Assert.Equal(r.Errors.First().Message,
                ErrorMessages.FailToGenRestorePassToken)
                );
        }

        [Fact]
        public async Task ReturnsFailure_WhenFailToSendEmail()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => { return new User(); });
            _userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(() => { return "SomeToken"; });

            _urlGenMock.Setup(ug => ug.Url(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(), It.IsAny<HttpContext>()))
                .Returns("someUrl");

            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<Message>()))
                .ReturnsAsync(() => false);

            // Act
            var r = await _handler.Handle(_request, CancellationToken.None);

            Assert.Multiple(
                () => Assert.True(r.IsFailed),
                () => Assert.Equal(r.Errors.First().Message, ErrorMessages.FailSendEmail));
        }

        [Fact]
        public async Task ReturnsSuccess_WhenEmailSendCorrectly()
        {
            var succRes = "Email for restoring password was sent successfuly.";

            // Arrange
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => { return new User(); });
            _userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(() => { return "SomeToken"; });

            _urlGenMock.Setup(ug => ug.Url(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(), 
                It.IsAny<HttpContext>()))
                .Returns("someUrl");

            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<Message>()))
                .ReturnsAsync(() => true);

            // Act
            var r = await _handler.Handle(_request, CancellationToken.None);

            Assert.Multiple(
                () => Assert.True(r.IsSuccess),
                () => Assert.Equal(succRes, r.Value));
        }
    }
}
