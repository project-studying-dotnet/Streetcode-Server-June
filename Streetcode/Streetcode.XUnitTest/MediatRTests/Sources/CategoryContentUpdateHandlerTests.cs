using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateContent;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Sources
{
    public class CategoryContentUpdateHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private StreetcodeCategoryContent _content = new StreetcodeCategoryContent() { Text = "str", SourceLinkCategoryId = 1, StreetcodeId = 2 };
        private StreetcodeCategoryContentDTO _contentDTO = new() { Text = "str", SourceLinkCategoryId = 1, StreetcodeId = 2 };

        private IEnumerable<StreetcodeCategoryContent> contents;

        private CategoryContentUpdateDTO _updateContentDTO = new()
        {
            Text = "NEWstr",
            SourceLinkCategoryId = 1,
            StreetcodeId = 2,
        };

        public CategoryContentUpdateHandlerTests()
        {
            contents = new List<StreetcodeCategoryContent>()
            {
                new StreetcodeCategoryContent()
                {
                    Text = "str",
                    SourceLinkCategoryId = 2,
                    StreetcodeId = 1,
                },
                new StreetcodeCategoryContent()
                {
                    Text = "Newstr",
                    SourceLinkCategoryId = 1,
                    StreetcodeId = 2,
                }
            };

            _mapperMock = new Mock<IMapper>();
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        public EntityEntry<StreetcodeCategoryContent> UpdateCat(StreetcodeCategoryContent content)
        {
            var res = contents.FirstOrDefault(u => u.SourceLinkCategoryId == content.SourceLinkCategoryId);
            if (res != null)
            {
                res.Text = content.Text;
            }

            return default;
        }

        [Fact]
        public async Task Handle_Should_ReturnEqualTrue_WhenInputNewCategory()
        {
            // Arrange
            var entityEntryMock = new Mock<EntityEntry<StreetcodeCategoryContent>>();
            _mapperMock.Setup(obj => obj.Map<StreetcodeCategoryContentDTO>(It.IsAny<object>())).Returns(_contentDTO);
            _mapperMock.Setup(obj => obj.Map<StreetcodeCategoryContent>(It.IsAny<object>())).Returns(_content);
            _wrapperMock.Setup(obj => obj.StreetcodeCategoryContentRepository.Update(It.IsAny<StreetcodeCategoryContent>())).Returns(UpdateCat(_content));

            var request = new CategoryContentUpdateCommand(_updateContentDTO);
            var handler = new CategoryContentUpdateHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() => Assert.True(result.IsSuccess), () => Assert.Equal(_content.Text, contents.ToList()[1].Text));
        }

        [Fact]
        public async Task Handle_Should_ReturnFailed_WhenInputEmptyCategory()
        {
            // Arrange
            var content = new StreetcodeCategoryContent() { Text = "str", SourceLinkCategoryId = 5, StreetcodeId = 5 };
            _mapperMock.Setup(obj => obj.Map<StreetcodeCategoryContentDTO>(It.IsAny<object>())).Returns(_contentDTO);
            _mapperMock.Setup(obj => obj.Map<StreetcodeCategoryContent>(It.IsAny<object>())).Returns(_content);
            _wrapperMock.Setup(obj => obj.StreetcodeCategoryContentRepository.Update(It.IsAny<StreetcodeCategoryContent>())).Returns(UpdateCat(new StreetcodeCategoryContent()));

            var request = new CategoryContentUpdateCommand(_updateContentDTO);
            var handler = new CategoryContentUpdateHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotEqual(_content.Text, contents.ToList()[1].Text);
        }
    }
}
