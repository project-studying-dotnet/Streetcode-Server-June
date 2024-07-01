using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.Register;
using Streetcode.BLL.MediatR.Media.Art.Update;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.Register
{
    public class RegisterUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly TokensConfiguration _tokensConfiguration;
        private readonly RegisterUserHandler _handler;

        public RegisterUserHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _tokensConfiguration = new TokensConfiguration();

            _handler = new RegisterUserHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object,
                _tokensConfiguration,
                _tokenServiceMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact]
        public async Task Handle_EmailAlreadyUsed_ReturnsFailResult()
        {
            // Arrange
            var request = new RegisterUserCommand(new UserRegisterDTO());
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("A user with this email is already registered", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_LoginAlreadyUsed_ReturnsFailResult()
        {
            // Arrange
            var request = new RegisterUserCommand(new UserRegisterDTO());
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("A user with this login is already registered", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_CreateUserFailed_ReturnsFailResult()
        {
            // Arrange
            var request = new RegisterUserCommand(new UserRegisterDTO ());
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(new User());
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("Failed to create user", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_AddToRoleFailed_ReturnsFailResult()
        {
            // Arrange
            var request = new RegisterUserCommand(new UserRegisterDTO ());
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(new User());
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("Failed to add role", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_Success_ReturnsSuccessResult()
        {
            // Arrange
            var request = new RegisterUserCommand(new UserRegisterDTO { Email = "test@test.com", Password = "Test@123" });
            var user = new User();
            var userDto = new UserDTO();
            var tokenResponse = new TokenResponseDTO
            {
                AccessToken = "accessToken",
                RefreshToken = new RefreshTokenDTO { Token = "refreshToken" }
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(user);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _tokenServiceMock.Setup(ts => ts.GenerateTokens(It.IsAny<User>())).ReturnsAsync(tokenResponse);
            _mapperMock.Setup(m => m.Map<UserDTO>(It.IsAny<User>())).Returns(userDto);

            var httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContext);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
