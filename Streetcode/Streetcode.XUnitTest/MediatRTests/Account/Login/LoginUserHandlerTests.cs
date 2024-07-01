using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.Login;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Xunit;
using Microsoft.AspNetCore.Authentication;

namespace Streetcode.XUnitTest.MediatRTests.Account.Login
{
    public class LoginUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly LoginUserHandler _handler;

        public LoginUserHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<User>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<User>>();

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object
            );

            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new LoginUserHandler(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenUserNotFound()
        {
            // Arrange
            var command = new LoginUserCommand(new UserLoginDTO { Username = "nonexistent", Password = "password" });
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidUsernameOrPassword, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new User { UserName = "SuperAdmin" };
            var command = new LoginUserCommand(new UserLoginDTO { Username = "SuperAdmin", Password = "wrongpassword" });
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.LoginUser.Password, false))
                .ReturnsAsync(SignInResult.Failed);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidUsernameOrPassword, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenMappingFails()
        {
            // Arrange
            var user = new User { UserName = "SuperAdmin" };
            var command = new LoginUserCommand(new UserLoginDTO { Username = "SuperAdmin", Password = "password" });
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.LoginUser.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _mapperMock.Setup(x => x.Map<UserDTO>(user)).Returns((UserDTO)null!);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenLoginIsSuccessful()
        {
            // Arrange
            var user = new User { UserName = "SuperAdmin" };
            var tokens = new TokenResponseDTO { AccessToken = "access_token", RefreshToken = new RefreshTokenDTO { Token = "refresh_token" } };
            var userDto = new UserDTO { Username = "SuperAdmin" };
            var command = new LoginUserCommand(new UserLoginDTO { Username = "SuperAdmin", Password = "password" });

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.LoginUser.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(x => x.GenerateTokens(user)).ReturnsAsync(tokens);
            _mapperMock.Setup(x => x.Map<UserDTO>(user)).Returns(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userDto, result.Value.User);
            Assert.Equal(tokens.AccessToken, result.Value.AccessToken);
            Assert.Equal(tokens.RefreshToken.Token, result.Value.RefreshToken.Token);
        }
    }
}
