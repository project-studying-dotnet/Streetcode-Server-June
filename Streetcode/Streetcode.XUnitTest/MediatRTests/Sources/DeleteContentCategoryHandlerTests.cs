using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteContentCategory;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;
using Streetcode.XUnitTest.MediatRTests.MapperConfigure;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Streetcode.XUnitTest.MediatRTests.SourcesTest
{
    public class DeleteContentCategoryHandlerTests
    {
        private List<StreetcodeCategoryContent> m_repo;
        private readonly IMapper m_mapper;
        private readonly Mock<ILoggerService> m_logger;

        private Mock<IRepositoryWrapper> m_repmock;

        public DeleteContentCategoryHandlerTests()
        {
            m_mapper = Mapper_Configurator.Create<StreetcodeCategoryContentProfile>();

            m_logger = new Mock<ILoggerService>();

            m_repo = new List<StreetcodeCategoryContent>()
            {
                new StreetcodeCategoryContent()
                {
                    StreetcodeId = 1,
                    SourceLinkCategoryId = 1,
                    Text = "some text"
                }
            };
        }

        [Fact]
        public async Task Handler_DeleteSuccess_WhenRepIsNotEmpty()
        {
            // Assign
            DeleteContentCategoryCommand quer = new DeleteContentCategoryCommand(1, 1);

            await SetupRepoWrapper(quer);

            var handler = new DeleteContentCategoryHandler(m_mapper, m_logger.Object, m_repmock.Object);

            // Act
            var result = await handler.Handle(quer, CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.Errors.Count() == 0),
                () => Assert.True(m_repo.Count() == 0));
        }

        [Fact]
        public async Task Handler_DeleteFail_WhenIdIsIncorrect()
        {
            DeleteContentCategoryCommand quer = new DeleteContentCategoryCommand(0, 0);

            await SetupRepoWrapper(quer);

            var handler = new DeleteContentCategoryHandler(m_mapper, m_logger.Object, m_repmock.Object);

            // Act
            var result = await handler.Handle(quer, CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.Errors.Count > 0),
                () => Assert.True(m_repo.Count() != 0));
        }

        [Fact]
        public async Task Handler_Delete_Fail_WhenRepositioryIsEmpty()
        {
            m_repo = new List<StreetcodeCategoryContent>();

            DeleteContentCategoryCommand quer = new DeleteContentCategoryCommand(1, 1);

            await SetupRepoWrapper(quer);

            var handler = new DeleteContentCategoryHandler(m_mapper, m_logger.Object, m_repmock.Object);

            var result = await handler.Handle(quer, CancellationToken.None);

            Assert.Multiple(
          () => Assert.True(result.Errors.Count > 0),
                () => Assert.Equal("Cannot find any Categories with corresponding id: 1", result.Errors[0].Message));
        }

        public void Remove(StreetcodeCategoryContent c)
        {
            m_repo.Remove(c);
        }

        public async Task<StreetcodeCategoryContent?> GetElementAsync(Func<StreetcodeCategoryContent, bool> predicate)
        {
            StreetcodeCategoryContent? con = null;

            await Task.Run(() =>
            {
                con = m_repo.Where(predicate).FirstOrDefault();
            });

            return con;
        }

        public async Task SetupRepoWrapper(DeleteContentCategoryCommand quer)
        {
            m_repmock = new Mock<IRepositoryWrapper>();
            m_repmock.Setup(r => r.StreetcodeCategoryContentRepository.
            GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeCategoryContent>, IIncludableQueryable<StreetcodeCategoryContent, object>>>()))
            .ReturnsAsync(await GetElementAsync(e => e.SourceLinkCategoryId == quer.sourcelinkcatId && e.StreetcodeId == quer.streetcodeId));

            m_repmock.Setup(r => r.StreetcodeCategoryContentRepository.
            Delete(It.IsAny<StreetcodeCategoryContent>())).Callback<StreetcodeCategoryContent>(
                c => Remove(c)).Verifiable();

            m_repmock.Setup(r => r.SaveChanges()).Returns(1);
        }
    }
}
