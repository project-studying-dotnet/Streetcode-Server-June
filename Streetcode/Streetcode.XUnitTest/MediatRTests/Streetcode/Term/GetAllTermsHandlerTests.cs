namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Term;

using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.XUnitTest.MediatRTests.MapperConfigure;

public class GetAllTermsHandlerTests
{
    private static IEnumerable<Term> m_Terms = new List<Term>()
        {
            new Term() { Id = 1 },
            new Term() { Id = 2 },
            new Term() { Id = 3 },
        };

    private readonly IMapper? m_Mapper;

    public GetAllTermsHandlerTests()
    {
        m_Mapper = Mapper_Configurator.Create<TermProfile>();
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

        GetAllTermsHandler handler = new GetAllTermsHandler(wrapperMock.Object, m_Mapper);

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

        Mock<ITermRepository> term_Rep_Mock = new Mock<ITermRepository>();
        term_Rep_Mock.Setup(trm => trm.GetAllAsync(default, default)).
            ReturnsAsync(new List<Term>());

        Mock<IRepositoryWrapper> wrapperMock = new Mock<IRepositoryWrapper>();
        wrapperMock.Setup(w => w.TermRepository).Returns(term_Rep_Mock.Object);

        GetAllTermsHandler handler = new GetAllTermsHandler(wrapperMock.Object, m_Mapper);

        // Act
        var result = await handler.Handle(querry, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Any());
    }
}
