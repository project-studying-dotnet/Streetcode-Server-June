using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Comments;

public class UpdateCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateCommentHandler _handler;

    public UpdateCommentHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new UpdateCommentHandler(
            _repositoryWrapperMock.Object,
            _loggerServiceMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFail()
    {
        // Arrange
        SetupHttpContextAccessor(Guid.Empty.ToString());
        var updateComment = new EditCommentDto { Id = 1 };
        var request = new UpdateCommentCommand(updateComment);
        var errorMsg = "User is not authenticated.";

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.True(result.Errors[0]?.Message.Contains(errorMsg));
    }

    [Fact]
    public async Task Handle_CommentNotFound_ReturnsFail()
    {
        // Arrange
        SetupHttpContextAccessor(Guid.NewGuid().ToString());
        SetupRepository(null!);
        var updateComment = new EditCommentDto { Id = 1 };
        var request = new UpdateCommentCommand(updateComment);
        var errorMsg = string.Format("Comment with id {0} not found.", updateComment.Id);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Contains(errorMsg, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_UnauthorizedAccessForEditComment_ReturnsFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId.ToString());
        var comment = new Comment { Id = 1, UserId = Guid.NewGuid() };
        SetupRepository(comment);
        var updateComment = new EditCommentDto { Id = comment.Id };
        var request = new UpdateCommentCommand(updateComment);
        var errorMsg = string.Format("User {0} does not have permission to edit this comment.", userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Contains(errorMsg, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_FailToUpdate_ReturnsFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId.ToString());
        var comment = new Comment { Id = 1, UserId = userId };
        SetupRepository(comment);
        SetupSaveChangesAsync(0);
        var updateComment = new EditCommentDto { Id = comment.Id };
        var request = new UpdateCommentCommand(updateComment);
        var errorMsg = string.Format("Fail to update comment with id {0}", comment.Id);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Contains(errorMsg, result.Errors[0].Message);
    }

    private void SetupHttpContextAccessor(string userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
    }

    private void SetupRepository(Comment comment)
    {
        var commentRepositoryMock = new Mock<ICommentRepository>();
        commentRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<Func<IQueryable<Comment>, IIncludableQueryable<Comment, object>>>()))
            .ReturnsAsync(comment);

        var entityEntryMock1 = new Mock<EntityEntry<Comment>>();
        entityEntryMock1.Setup(e => e.Entity).Returns(comment);

        commentRepositoryMock.Setup(x => x.Update(comment))
            .Returns(It.IsAny<EntityEntry<Comment>>());

        _repositoryWrapperMock.Setup(x => x.CommentRepository).Returns(commentRepositoryMock.Object);
    }

    private void SetupSaveChangesAsync(int result)
    {
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(result);
    }
}