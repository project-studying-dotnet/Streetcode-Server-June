using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAll;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.StreetcodeTest;

public class GetAllStreetcodesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllStreetcodesHandler _handler;

    public GetAllStreetcodesHandlerTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllStreetcodesHandler(_mockRepoWrapper.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_StreetcodesNull_ReturnsFailResult()
    {
        // Arrange
        var msgError = "Cannot find any";
        var request = GetSampleRequest();
        var query = new GetAllStreetcodesQuery(request);
        _mockRepoWrapper.Setup(repo => repo.StreetcodeRepository.FindAll(null)).Returns((IQueryable<StreetcodeContent>)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Contains(msgError, result.Errors?.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_InvalidPaginationParameters_ReturnsFailResult()
    {
        // Arrange
        var msgError = "Amount and page must be greater than zero";
        var request = GetSampleRequest(0, 0);
        var query = new GetAllStreetcodesQuery(request);
        var streetcodes = GetSampleStreetcodes().AsQueryable();
        _mockRepoWrapper.Setup(repo => repo.StreetcodeRepository.FindAll(null)).Returns(streetcodes);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(msgError, result.Errors?.FirstOrDefault()?.Message);
    }

    private List<StreetcodeContent> GetSampleStreetcodes()
    {
        var streetcodes = new List<StreetcodeContent>
        {
            new StreetcodeContent { Id = 1, Title = "Streetcode 1", Status = StreetcodeStatus.Published },
            new StreetcodeContent { Id = 2, Title = "Streetcode 2", Status = StreetcodeStatus.Published },
            new StreetcodeContent { Id = 3, Title = "Streetcode 3", Status = StreetcodeStatus.Deleted }
        };

        return streetcodes;
    }

    private GetAllStreetcodesRequestDTO GetSampleRequest(int amount = 10, int page = 1, string title = "", string sort = "", string filter = "")
    {
        return new GetAllStreetcodesRequestDTO
        {
            Amount = amount,
            Page = page,
            Title = title,
            Sort = sort,
            Filter = filter
        };
    }

    private List<StreetcodeDTO> GetSampleStreetcodeDTOs()
    {
        var streetcodeDTOs = new List<StreetcodeDTO>
        {
            new StreetcodeDTO { Id = 1, Title = "Streetcode 1", Status = StreetcodeStatus.Published },
            new StreetcodeDTO { Id = 2, Title = "Streetcode 2", Status = StreetcodeStatus.Published },
            new StreetcodeDTO { Id = 3, Title = "Streetcode 3", Status = StreetcodeStatus.Deleted }
        };

        return streetcodeDTOs;
    }
}