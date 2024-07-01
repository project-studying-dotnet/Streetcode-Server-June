using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.Register;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.Register
{
    public class RegisterUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly RegisterUserHandler _handler;

        public RegisterUserHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new RegisterUserHandler(_mapperMock.Object, _loggerMock.Object, _userManagerMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsError_When_EmailIsAlreadyInUse()
        {
            // Arrange
            var command = new RegisterUserCommand(new UserRegisterDTO());
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("A user with this email is already registered.", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_CreateUserFails_ReturnsFail()
        {
            // Arrange
            var command = new RegisterUserCommand(new UserRegisterDTO());
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mapperMock.Setup(x => x.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(new User());
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Failed to create user", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_AddRoleFails_ReturnsFail()
        {
            // Arrange
            var command = new RegisterUserCommand(new UserRegisterDTO { Email = "test@example.com", Login = "testlogin", Password = "password" });
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mapperMock.Setup(x => x.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(new User());
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "USER")).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Failed to add role", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_Success_ReturnsOk()
        {
            // Arrange
            var command = new RegisterUserCommand(new UserRegisterDTO { Email = "test@example.com", Login = "testlogin", Password = "password" });
            var user = new User();
            var userDTO = new UserDTO();
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _mapperMock.Setup(x => x.Map<User>(It.IsAny<UserRegisterDTO>())).Returns(user);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "USER")).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
