using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.GetAllByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.WebApi.Controllers.Comment;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Comments
{
    public class GetAllCommentsByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public GetAllCommentsByStreetcodeIdHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnFilledList_WhenGetRightStreetcodeId()
        {
            // Arrange
            int streetcodeId = 1;
            List<CommentDTO> dtos = new()
            {
                new CommentDTO
                {
                    Id = 1,
                    StreetcodeId = 1,
                    CommentContent = "Content",
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            };
            MockSetup(false, dtos);
            var handlerObj = new GetAllCommentsByStreetcodeIdHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handlerObj.Handle(new GetAllCommentsByStreetcodeIdQuery(streetcodeId), default);

            // Assert
            Assert.Multiple(
            () => Assert.Equal(dtos, result.Value),
            () => Assert.True(result.Value.All(c => c.StreetcodeId == streetcodeId)));
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_WhenStreetcodeIdNotFound()
        {
            // Arrange
            int streetcodeId = 99;
            MockSetup(true);
            var handlerObj = new GetAllCommentsByStreetcodeIdHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handlerObj.Handle(new GetAllCommentsByStreetcodeIdQuery(streetcodeId), default);

            // Assert
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenGetsNull()
        {
            // Arrange
            int streetcodeId = 1;
            MockSetupNull();
            var handlerObj = new GetAllCommentsByStreetcodeIdHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
            var request = new GetAllCommentsByStreetcodeIdQuery(streetcodeId);

            // Act
            var result = await handlerObj.Handle(request, default);

            // Assert
            Assert.True(result.Errors[0].Message == 
                MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request));
        }

        private void MockSetup(bool returnEmpty, List<CommentDTO>? dtos = null)
        {
            Guid guid = Guid.NewGuid();
            DateTime date = DateTime.UtcNow;

            List<Comment> comments = new()
            {
                new Comment
                {
                    Id = 1,
                    StreetcodeId = 1,
                    CommentContent = "Content",
                    UserId = guid,
                    CreatedAt = date
                }
            };

            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<CommentDTO>>(It.IsAny<IEnumerable<Comment>>())).Returns(returnEmpty ? new List<CommentDTO>() : dtos);
            _wrapperMock.Setup(obj => obj.CommentRepository.GetAllAsync(default, default)).ReturnsAsync(returnEmpty ? new List<Comment>() : comments);
        }

        private void MockSetupNull()
        {
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<CommentDTO>>(It.IsAny<IEnumerable<Comment>>())).Returns((List<CommentDTO>)null!);
            _wrapperMock.Setup(obj => obj.CommentRepository.GetAllAsync(
                It.IsAny<Expression<Func<Comment, bool>>>(),
                It.IsAny<Func<IQueryable<Comment>, IIncludableQueryable<Comment, object>>>()))
                .ReturnsAsync((List<Comment>)null!);
        }
    }
}
