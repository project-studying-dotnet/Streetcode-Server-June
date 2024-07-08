using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Mapping.Users;
using Streetcode.BLL.MediatR.Account.Delete;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.DAL.Entities.Users;
using Streetcode.XUnitTest.MediatRTests.MapperConfigure;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.DeleteTests
{
    public class CustTrueIdentityResult : IdentityResult
    {
        public CustTrueIdentityResult()
        {
            Succeeded = true;
        }
    }
    
    public class DeleteUserHandlerTests
    {
        private DeleteUserCommandHandler _handler;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ICookieService> _cookieServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        private List<User> _userDb;
        
        public DeleteUserHandlerTests()
        {
            _userDb = new List<User>()
            {
                new User()
                {
                    Id = Guid.Parse("d8e4e3c9-4b6a-4418-acaf-06011d158bd5"), 
                    UserName = "test", Email = "test@gmail.com",
                    FirstName = "Test", LastName = "Test"
                }
            };
            _userDb[0].PasswordHash = new PasswordHasher<User>().HashPassword(_userDb[0], "supper");

            _mapper = Mapper_Configurator.Create<UserProfile>();   
            _loggerServiceMock = new Mock<ILoggerService>();

            var userStore = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _cacheServiceMock = new Mock<ICacheService>();
            _cookieServiceMock = new Mock<ICookieService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _handler = new DeleteUserCommandHandler(
                _mapper,
                _loggerServiceMock.Object,
                _userManagerMock.Object,
                _httpContextAccessorMock.Object,
                _cacheServiceMock.Object,
                _cookieServiceMock.Object,
                _tokenServiceMock.Object
                );
        }

        [Fact]
        public async Task Handle_ReturnError_WhenThereIsNoAccessToken()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies.TryGetValue(It.IsAny<string>(), out It.Ref<string>.IsAny!))
                .Returns((string key, out string value) => { value = null; return false; });     
            
            // Act
            var r = await _handler.Handle(new DeleteUserCommand(), CancellationToken.None);
            
            // Assert
            Assert.Multiple(
                () => Assert.True(r.Errors.Any()),
                () => Assert.Equal(r.Errors.First().Message, ErrorMessages.AccessTokenNotFound));
        }

        [Fact]
        public async Task Handle_ReturnError_WhenThereIsEmptyAccessToken()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies.TryGetValue(It.IsAny<string>(), out It.Ref<string>.IsAny!))
                .Returns((string key, out string value) => { value = null; return true; });

            // Act
            var r = await _handler.Handle(new DeleteUserCommand(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(r.IsFailed),
                () => Assert.Equal(r.Errors.First().Message, ErrorMessages.AccessTokenNotFound));
        }
        
        [Fact]
        public async Task Handle_ReturnError_WhenIdOfUserInAccessTokenIsIncorrect()
        {
            // Arrange
            string id = "someIncorrectId";

            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies.TryGetValue(It.IsAny<string>(), out It.Ref<string>.IsAny!))
                .Returns((string key, out string value) => { value = "someToken"; return true; });

            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>()))
                .Returns(id);

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => { return null; });

            // Act
            var r = await _handler.Handle(new DeleteUserCommand(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(r.IsFailed),
                () => Assert.Equal(r.Errors.First().Message, string.Format(ErrorMessages.UserWithIdNotFound, id)));
        }

        [Fact]
        public async Task Handle_ReturnError_WhenDeletingOfUserFailed()
        {
            // Arrange
            string id = "someId";

            string errorMsg = string.Format(ErrorMessages.FailToDeleteA, "User");

            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies.TryGetValue(It.IsAny<string>(), out It.Ref<string>.IsAny!))
                .Returns((string key, out string value) => { value = "someToken"; return true; });

            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>()))
               .Returns(id);

            IdentityResult identityResult = new IdentityResult();
            
            identityResult.GetType().GetProperty("Succeeded").GetSetMethod(true).Invoke(identityResult, new object[] { false });
            
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(identityResult);

            // Act

            var r = await _handler.Handle(new DeleteUserCommand(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(r.IsFailed));
        }

        [Fact]
        public async Task Handle_ReturnError_WhenSettingToBlackListFails()
        {
            // Arrange
            var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshTokenList = new List<DAL.Entities.Users.RefreshToken>
            {
                new DAL.Entities.Users.RefreshToken
                {
                    Token = refreshTokenValue,
                    Expires = DateTime.UtcNow.AddDays(2)
                }
            };

            _userDb[0].RefreshTokens = refreshTokenList;

            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies
            .TryGetValue(It.IsAny<string>(), out It.Ref<string>.IsAny!))
                .Returns((string key, out string value) =>
                {
                    value = "ValidAccessToken";

                    return true;
                });
           
            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies.Keys)
                .Returns(new List<string>() { "accessToken", });

            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>()))
                .Returns(_userDb[0].Id.ToString());

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_userDb[0]);
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => { _userDb.Remove(u); return new CustTrueIdentityResult(); });

            _cacheServiceMock.Setup(x => x.SetBlacklistedTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var r = await _handler.Handle(new DeleteUserCommand(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(r.IsFailed),
                () => Assert.Equal(ErrorMessages.FailedToSetTokenInBlackList, r.Errors.First().Message));
        }

        [Fact]
        public async Task Handle_ShouldSuccess_WhenDeleteUserIsSuccessful()
        {
            // Arrange
            var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshTokenList = new List<DAL.Entities.Users.RefreshToken>
            {
                new DAL.Entities.Users.RefreshToken
                {
                    Token = refreshTokenValue,
                    Expires = DateTime.UtcNow.AddDays(2)
                }
            };

            _userDb[0].RefreshTokens = refreshTokenList;

            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies
            .TryGetValue(It.IsAny<string>(), out It.Ref<string>.IsAny!))
                .Returns((string key, out string value) => 
                {
                    value = "ValidAccessToken";

                    return true;
                });
            
            _httpContextAccessorMock.Setup(x => x.HttpContext.Request.Cookies.Keys)
                .Returns(new List<string>() { "accessToken" });

            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>()))
                .Returns(_userDb[0].Id.ToString());
            
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_userDb[0]);
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => { _userDb.Remove(u); return new CustTrueIdentityResult(); });

            _cacheServiceMock.Setup(x => x.SetBlacklistedTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            
            // Act
            var r = await _handler.Handle(new DeleteUserCommand(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(r.IsSuccess),
                () => Assert.False(_userDb.Any()),
                () => Assert.Equal("Test", r.Value.Firstname),
                () => Assert.Equal("Test", r.Value.Lastname));
        }
    }
}
