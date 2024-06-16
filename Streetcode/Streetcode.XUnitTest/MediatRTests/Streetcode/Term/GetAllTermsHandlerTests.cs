using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.XUnitTest.MediatRTests.MapperConfigure;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Term;

public class GetAllTermsHandlerTests
    {
        private static IEnumerable<DAL.Entities.Streetcode.TextContent.Term> m_Terms = new List<DAL.Entities.Streetcode.TextContent.Term>()
        {
            new DAL.Entities.Streetcode.TextContent.Term() { Id = 1 },
            new DAL.Entities.Streetcode.TextContent.Term() { Id = 2 },
            new DAL.Entities.Streetcode.TextContent.Term() { Id = 3 },
        };

        private readonly IMapper? m_Mapper;

        private readonly Mock<ILoggerService> m_loggerMock;

        public GetAllTermsHandlerTests()
        {
            m_Mapper = Mapper_Configurator.Create<TermProfile>();

            m_loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task GetAllTerms_ShouldReturn_A_Collection_Of_Terms()
        {
            // Assign
            GetAllTermsQuery querry = new GetAllTermsQuery();

            Mock<ITermRepository> term_Rep_Mock = new Mock<ITermRepository>();
            term_Rep_Mock.Setup(trm => trm.GetAllAsync(default, default)).
                ReturnsAsync(m_Terms);

            Mock<IRepositoryWrapper> wrapperMock = new Mock<IRepositoryWrapper>();
            wrapperMock.Setup(w => w.TermRepository).Returns(term_Rep_Mock.Object);

            m_loggerMock.Setup(l => l.LogError(querry, "Cannot find any term!"));

            GetAllTermsHandler handler = new GetAllTermsHandler(wrapperMock.Object, m_Mapper, m_loggerMock.Object);

            // Act
            var result = await handler.Handle(querry, CancellationToken.None);

            // Assert
            Assert.True(result.Value.Count() == m_Terms.Count());
        }

        [Fact]
        public async Task GetAllTerms_CollectionIsEmpty_ShouldReturnEmptyCollection()
        {
            // Assign
            GetAllTermsQuery querry = new GetAllTermsQuery();

            m_loggerMock.Setup(l => l.LogError(querry, "Cannot find any term!"));

            Mock<ITermRepository> term_Rep_Mock = new Mock<ITermRepository>();
            term_Rep_Mock.Setup(trm => trm.GetAllAsync(default, default)).
                ReturnsAsync(new List<DAL.Entities.Streetcode.TextContent.Term>());

            Mock<IRepositoryWrapper> wrapperMock = new Mock<IRepositoryWrapper>();
            wrapperMock.Setup(w => w.TermRepository).Returns(term_Rep_Mock.Object);

            GetAllTermsHandler handler = new GetAllTermsHandler(wrapperMock.Object, m_Mapper, m_loggerMock.Object);

            // Act
            var result = await handler.Handle(querry, CancellationToken.None);

            // Assert
            Assert.True(result.Value.Count() == 0);
        }

        [Fact]
        public async Task GetAllTerms_CollectionIsNull_ShouldReturnError()
        {
            // Assign
            GetAllTermsQuery querry = new GetAllTermsQuery();

            m_loggerMock.Setup(l => l.LogError(querry, "Cannot find any term!"));

            Mock<ITermRepository> term_Rep_Mock = new Mock<ITermRepository>();
            term_Rep_Mock.Setup(trm => trm.GetAllAsync(default, default)).
                ReturnsAsync(() => null);

            Mock<IRepositoryWrapper> wrapperMock = new Mock<IRepositoryWrapper>();
            wrapperMock.Setup(w => w.TermRepository).Returns(term_Rep_Mock.Object);

            GetAllTermsHandler handler = new GetAllTermsHandler(wrapperMock.Object, m_Mapper, m_loggerMock.Object);

            // Act
            var result = await handler.Handle(querry, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }
    }
