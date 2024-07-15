using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Moq.EntityFrameworkCore;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Comments
{
    public class DeleteCommentHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly DeleteCommentHandler _handler;

        public DeleteCommentHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new DeleteCommentHandler(_repositoryWrapperMock.Object, _loggerMock.Object, _mapperMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Handle_CommentNotFound_ShouldReturnFailResult()
        {
            // Arrange
            var request = new DeleteCommentCommand(1);
            _repositoryWrapperMock.Setup(r => r.CommentRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Comment, bool>>>(), default))
                                  .ReturnsAsync((Comment)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request), result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_UserNotAuthorized_ShouldReturnFailResult()
        {
            // Arrange
            var request = new DeleteCommentCommand(1);
            var comment = new Comment { Id = 1 };
            _repositoryWrapperMock.Setup(r => r.CommentRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Comment, bool>>>(), default))
                                  .ReturnsAsync(comment);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = user });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request), result.Errors[0].Message);
        }
    }
}
