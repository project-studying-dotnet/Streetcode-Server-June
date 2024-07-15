using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.GetAll;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Comments
{
    public class GetAllCommentsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public GetAllCommentsHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnEqualTrue_WhenGetTrueComment()
        {
            // Arrange
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
            var handlerObj = new GetAllCommentsHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handlerObj.Handle(new GetAllCommentsQuery(), default);

            // Assert
            Assert.Equal(dtos, result.Value);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmpty_WhenGetEmptyComment()
        {
            // Arrange
            MockSetup(true);
            var handlerObj = new GetAllCommentsHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handlerObj.Handle(new GetAllCommentsQuery(), default);

            // Assert
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenGetsNullComments()
        {
            // Arrange
            MockSetupNull();
            var handlerObj = new GetAllCommentsHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handlerObj.Handle(new GetAllCommentsQuery(), default);

            // Assert
            Assert.Equal(result.Errors[0].Message, MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, new GetAllCommentsQuery()));
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
            _wrapperMock.Setup(obj => obj.CommentRepository.GetAllAsync(default, default)).ReturnsAsync((List<Comment>)null!);
        }
    }
}
