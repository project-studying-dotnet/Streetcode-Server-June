using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Likes.PushLike;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Likes;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Likes
{
    public class PushLikeHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private StreetcodeContent _streetcodeContent = new()
        {
            LikesCount = 0,
            Id = 1
        };
        PushLikeDTO pushLike = new()
        {
            UserId = Guid.NewGuid(),
            streetcodeId = 12
        };

        public PushLikeHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _mapperMock = new Mock<IMapper>();
            _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task Handler_ShouldReturnFailure_WhenUserManagerReturnNullUser()
        {
            // Arrange
            var request = new PushLikeCommand(pushLike);
            var handler = new PushLikeHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _userManagerMock.Object);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetFirstOrDefaultAsync(default, default)).ReturnsAsync(_streetcodeContent);
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
        public async Task Handler_ShouldReturnFailure_WhenStreetCodeIsNull()
        {
            // Arrange
            var request = new PushLikeCommand(pushLike);
            var handler = new PushLikeHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _userManagerMock.Object);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.StreetcodeNotExist);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetFirstOrDefaultAsync(default, default)).ReturnsAsync((StreetcodeContent)null!);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

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
        public async Task Handler_ShouldCreateLike_WhenLikeIsNull()
        {
            // Arrange
            var request = new PushLikeCommand(pushLike);
            var handler = new PushLikeHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _userManagerMock.Object);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), default)).ReturnsAsync(_streetcodeContent);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _wrapperMock.Setup(obj => obj.LikeRepository.GetFirstOrDefaultAsync(default, default)).ReturnsAsync((Like)null!);
            _wrapperMock.Setup(obj => obj.SaveChangesAsync()).ReturnsAsync(1);
            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(1, _streetcodeContent.LikesCount);
            });
        }

        [Fact]
        public async Task Handler_ShouldRemoveLike_WhenLikeIsNotNull()
        {
            // Arrange
            var request = new PushLikeCommand(pushLike);
            var handler = new PushLikeHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _userManagerMock.Object);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), default)).ReturnsAsync(_streetcodeContent);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _wrapperMock.Setup(obj => obj.LikeRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Like, bool>>>(), default)).ReturnsAsync(new Like());
            _wrapperMock.Setup(obj => obj.SaveChangesAsync()).ReturnsAsync(1);
            _streetcodeContent.LikesCount = 1;

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(0, _streetcodeContent.LikesCount);
            });
        }

        [Fact]
        public async Task Handler_ShouldError_WhenSaveIsNotSuccess()
        {
            // Arrange
            var request = new PushLikeCommand(pushLike);
            var handler = new PushLikeHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object, _userManagerMock.Object);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateNewLike);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), default)).ReturnsAsync(_streetcodeContent);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _wrapperMock.Setup(obj => obj.LikeRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Like, bool>>>(), default)).ReturnsAsync(new Like());
            _wrapperMock.Setup(obj => obj.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccess);
                Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
            });
        }
    }
}
