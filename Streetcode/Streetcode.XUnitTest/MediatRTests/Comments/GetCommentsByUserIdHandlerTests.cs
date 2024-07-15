using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.GetByUserId;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specification.Comment;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Comments
{
    public class GetCommentsByUserIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        private readonly GetCommentsByUserIdHandler _handler;
        private readonly GetCommentsByUserIdQuery _query;
        private readonly Guid _userId;
        private readonly IEnumerable<Comment> _list;
        private readonly IEnumerable<CommentDTO> _mappedList;

        public GetCommentsByUserIdHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _userId = Guid.NewGuid();
            _handler =
                new GetCommentsByUserIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
            _query = new GetCommentsByUserIdQuery(_userId);
            _list = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    UserId = _userId,
                }
            };
            _mappedList = new List<CommentDTO>()
            {
                new CommentDTO()
                {
                    Id = 1,
                    UserId = _userId,
                }
            };
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<CommentDTO>>(_list))
           .Returns(_mappedList);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenRepositoryHasCorrectParameters()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.CommentRepository
                .GetAllWithSpecAsync(It.IsAny<CommentsByUserIdSpec>()))
                .ReturnsAsync(_list);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            Assert.Multiple(
           () => Assert.True(result.IsSuccess),
           () => _mockRepositoryWrapper.Verify(repo => repo.CommentRepository.GetAllWithSpecAsync(
                    It.IsAny<CommentsByUserIdSpec>())));
        }

        [Fact]
        public async Task Handle_Should_ReturnMappedRelatedFigureDTO_WhenRepositoryReturnsData()
        {
            // Arrange
            _mockRepositoryWrapper.Setup(repo => repo.CommentRepository
                 .GetAllWithSpecAsync(It.IsAny<CommentsByUserIdSpec>()))
                 .ReturnsAsync(_list);

            // Act
            var result = await _handler.Handle(_query, CancellationToken.None);

            // Assert
            Assert.Multiple(
           () => Assert.True(result.IsSuccess),
           () => Assert.Equal(_mappedList, result.Value));
        }
    }
}
