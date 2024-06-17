using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.DTO.Sources.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Sources
{
    public class CreateSourceLinkCategoryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private SourceLinkCategoryDTO _categoryDTO = new SourceLinkCategoryDTO() { Id = 6, Title = "testing", ImageId = 2 };
        private SourceLinkCategory _category = new() { Id = 6, Title = "testing", ImageId = 2 };

        private CreateSourceCategoryDTO _createCategoryDto = new()
        {
            Id = 6,
            Title = "testing",
            ImageId = 2,
        };

        public CreateSourceLinkCategoryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnEqualTrue_WhenInputNewCategory()
        {
            // Arrange
            _mapperMock.Setup(obj => obj.Map<SourceLinkCategoryDTO>(It.IsAny<object>())).Returns(_categoryDTO);
            _mapperMock.Setup(obj => obj.Map<SourceLinkCategory>(It.IsAny<object>())).Returns(_category);
            _wrapperMock.Setup(obj => obj.SourceCategoryRepository.CreateAsync(_category)).ReturnsAsync(_category);

            var request = new CreateSourceLinkCategoryCommand(_createCategoryDto);
            var handler = new CreateSourceLinkCategoryHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result.Result.Value.Id);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailed_WhenInputEmptyCategory()
        {
            // Arrange
            _mapperMock.Setup(obj => obj.Map<SourceLinkCategoryDTO>(It.IsAny<object>())).Returns(new SourceLinkCategoryDTO());
            _mapperMock.Setup(obj => obj.Map<SourceLinkCategory>(It.IsAny<object>())).Returns(new SourceLinkCategory());
            _wrapperMock.Setup(obj => obj.SourceCategoryRepository.CreateAsync(new SourceLinkCategory())).ReturnsAsync(new SourceLinkCategory());

            var request = new CreateSourceLinkCategoryCommand(new CreateSourceCategoryDTO());
            var handler = new CreateSourceLinkCategoryHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Result.IsSuccess);
        }
    }
}
