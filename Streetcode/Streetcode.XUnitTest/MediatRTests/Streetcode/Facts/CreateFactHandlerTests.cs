using AutoMapper;
using FluentResults;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTest.Facts;

 public class CreateFactHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CreateFactHandler _handler;
    private readonly Fact _factEntity;
    private readonly FactDto _factDTO;
    private readonly Fact _withoutStreetcodeIdEntity;
    private readonly CreateFactCommand _createFactCommand;

    public CreateFactHandlerTests()
    {
        _factEntity = new Fact
        {
            Title = "Interesting Fact",
            FactContent = "This is an interesting fact about something.",
            ImageId = 1,
            StreetcodeId = 1,
        };

        _factDTO = new FactDto
        {
            Id = 1,
            Title = "Interesting Fact",
            ImageId = 1,
            FactContent = "This is an interesting fact about something very important.",
            StreetcodeId = 1
        };

        _withoutStreetcodeIdEntity = new Fact
        {
            Id = 1,
            Title = "Interesting Fact",
            ImageId = 1,
            FactContent = "This is an interesting fact about something very important.",
            StreetcodeId = 0
        };

        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _createFactCommand = new CreateFactCommand(_factDTO);
        _handler = new CreateFactHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFactDto_WhenFactIsCreatedSuccessfully()
    {
        // Arrange
        SetupMocksForSuccess();

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<FactDto>(_factEntity)).Returns(_factDTO);

        // Act
        var result = await _handler.Handle(_createFactCommand, CancellationToken.None);

        // Assert
        Assert.Equal(_factDTO, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenFactIsNull()
    {
        // Arrange
        var erorrMsg = new Error("New fact cannot be null");
        _mockMapper.Setup(m => m.Map<Fact>(_factDTO)).Returns((Fact)null!);

        // Act
        var result = await _handler.Handle(_createFactCommand, CancellationToken.None);

        // Assert
        Assert.True(result.Errors?.Exists(e => e.Message == erorrMsg.Message));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenStreetcodeIdZero()
    {
        // Arrange
        var erorrMsg = new Error("StreetcodeId cannot be 0. Please provide a valid StreetcodeId.");
        _mockMapper.Setup(m => m.Map<Fact>(_factDTO)).Returns(_withoutStreetcodeIdEntity);

        // Act
        var result = await _handler.Handle(_createFactCommand, CancellationToken.None);

        // Assert
        Assert.True(result.Errors?.Exists(e => e.Message == erorrMsg.Message));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenSaveChangesFails()
    {
        var erorrMsg = new Error("Failed to create a fact");

        // Arrange
        SetupMocksForSuccess();
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(_createFactCommand, CancellationToken.None);

        // Assert
        Assert.True(result.Errors?.Exists(e => e.Message == erorrMsg.Message));
    }

    private void SetupMocksForSuccess()
    {
        _mockMapper.Setup(m => m.Map<Fact>(_factDTO)).Returns(_factEntity);
        _mockRepositoryWrapper.Setup(r => r.FactRepository.CreateAsync(_factEntity))
            .Callback<Fact>(fact =>
            {
                fact.Id = 1;
            }).ReturnsAsync(_factEntity);
    }
}