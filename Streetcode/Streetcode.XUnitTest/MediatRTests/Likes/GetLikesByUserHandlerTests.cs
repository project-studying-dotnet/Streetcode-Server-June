using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Likes.PushLike;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Likes;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using Streetcode.BLL.Services.Tokens;
using Azure.Core;
using Castle.Core.Configuration;
using Streetcode.BLL.DTO.Streetcode;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.XUnitTest.MediatRTests.Likes
{
    public class GetLikesByUserHandlerTests
    {
        private static Guid _id = Guid.NewGuid();

        private readonly List<Like> _likes = new List<Like>()
        {
            new Like()
            {
                Id = 1,
                UserId = _id,
                Streetcode = new StreetcodeContent()
                {
                    Id = 1
                },
                streetcodeId = 1
            },
            new Like()
            {
                Id = 2,
                UserId = Guid.Empty,
                Streetcode = new StreetcodeContent()
                {
                    Id = 2
                },
                streetcodeId = 2
            },
            new Like()
            {
                Id = 3,
                UserId = _id,
                Streetcode = new StreetcodeContent()
                {
                    Id = 3
                },
                streetcodeId = 3
            }
        };

        private readonly List<StreetcodeDTO> _streetcodes = new List<StreetcodeDTO>()
        {
            new StreetcodeDTO()
            {
                Id = 1
            },
            new StreetcodeDTO()
            {
                Id = 3
            },
        };
        private Mock<IRepositoryWrapper> _wrapperMock;
        private Mock<ILoggerService> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<UserManager<User>> _userManagerMock;


        public GetLikesByUserHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _tokenServiceMock = new Mock<ITokenService>();
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task Handler_ShouldReturnFailure_WhenNoAccessToken()
        {
            // Arrange
            var handler = new GetLikesByUserHandler(
                _wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object,
                _tokenServiceMock.Object, _userManagerMock.Object);
            var request = new GetLikesByUserQuery();
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);

            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(false);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccess);
                Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
            });
        }

        [Fact]
        public async Task Handler_ShouldReturnFailure_WhenUserManagerReturnNullUser()
        {
            // Arrange
            var handler = new GetLikesByUserHandler(
               _wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object,
               _tokenServiceMock.Object, _userManagerMock.Object);
            var request = new GetLikesByUserQuery();
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
            _tokenServiceMock.Setup(ts => ts.GetUserIdFromAccessToken(It.IsAny<string>())).Returns(string.Empty);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccess);
                Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
            });
        }

        [Fact]
        public async Task Handler_ShouldReturnSuccess_WhenCorrectUser()
        {
            // Arrange
            var handler = new GetLikesByUserHandler(
               _wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object,
               _tokenServiceMock.Object, _userManagerMock.Object);
            var request = new GetLikesByUserQuery();
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
            _tokenServiceMock.Setup(ts => ts.GetUserIdFromAccessToken(It.IsAny<string>())).Returns(string.Empty);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _wrapperMock.Setup(obj => obj.LikeRepository.GetAllAsync(default, default)).ReturnsAsync(new List<Like>());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handler_ShouldReturnCorrectLikes_WhenCorrectUser()
        {
            // Arrange
            var handler = new GetLikesByUserHandler(
               _wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object,
               _tokenServiceMock.Object, _userManagerMock.Object);
            var request = new GetLikesByUserQuery();
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
            _tokenServiceMock.Setup(ts => ts.GetUserIdFromAccessToken(It.IsAny<string>())).Returns(string.Empty);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() {Id = _id });
            _wrapperMock.Setup(wrapper => wrapper.LikeRepository.GetAllAsync(
            It.IsAny<Expression<Func<Like, bool>>>(),
            It.IsAny<Func<IQueryable<Like>, IIncludableQueryable<Like, object>>>())).ReturnsAsync(_likes.Where(l => l.UserId == _id));
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<StreetcodeDTO>>(It.IsAny<IEnumerable<object>>())).Returns(_streetcodes);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(_streetcodes.Count, result.Value.Count());
            }); 
        }
    }
}
