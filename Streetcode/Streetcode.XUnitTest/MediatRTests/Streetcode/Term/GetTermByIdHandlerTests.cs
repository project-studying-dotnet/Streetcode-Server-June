using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.XUnitTest.MediatRTests.MapperConfigure;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Term;

public class GetTermByIdHandlerTests
{
    private static IEnumerable<Entity> m_Terms = new List<Entity>()
    {
        new Entity() { Id = 1 },
        new Entity() { Id = 2 },
        new Entity() { Id = 3 },
    };

    private readonly IMapper? m_Mapper;

    private readonly Mock<ILoggerService> m_loggerMock;

    private readonly Mock<IRepositoryWrapper> m_RepWrapperMock;

    private readonly Mock<ILoggerService> m_logger_mock;

    public GetTermByIdHandlerTests()
    {
        m_Mapper = Mapper_Configurator.Create<TermProfile>();

        m_loggerMock = new Mock<ILoggerService>();

        m_RepWrapperMock = new Mock<IRepositoryWrapper>();

        m_logger_mock = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task Handler_ShouldReturn_TermDTO_When_TermRepoIsNotEmpty(int id)
    {
        // Assign
        m_RepWrapperMock.Setup(x => x.TermRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Entity, bool>>>(),
               It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(GetTermById(e => e.Id == id));

        var request = new GetTermByIdQuery(id);

        var handler = new GetTermByIdHandler(m_RepWrapperMock.Object, m_Mapper, m_logger_mock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result<TermDTO>>(result),
            () => Assert.True(result.Value.Id == id));
    }

    [Fact]
    public async Task Handler_ShouldReturn_Error_When_ThereIsNoTermwithSuchId()
    {
        // Assign
        m_RepWrapperMock.Setup(rw => rw.TermRepository
        .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Entity, bool>>>(),
               It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(GetTermById(e => e.Id == m_Terms.Count() + 1));

        var query = new GetTermByIdQuery(m_Terms.Count() + 1);

        var handler = new GetTermByIdHandler(m_RepWrapperMock.Object, m_Mapper, m_logger_mock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assign
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handler_ReturnError_WhenRepoIsNull()
    {
        // Assign
        m_RepWrapperMock.Setup(rw => rw.TermRepository
        .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Entity, bool>>>(),
               It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(() => null);

        var query = new GetTermByIdQuery(1);

        var handler = new GetTermByIdHandler(m_RepWrapperMock.Object, m_Mapper, m_logger_mock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assign
        Assert.False(result.IsSuccess);
    }

    private Entity? GetTermById(Func<Entity, bool>? pred)
    {
        return m_Terms.Where(pred).FirstOrDefault();
    }
}
