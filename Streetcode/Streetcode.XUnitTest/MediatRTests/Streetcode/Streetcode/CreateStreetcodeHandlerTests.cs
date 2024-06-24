using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTest.StreetcodeBlock
{
    public class CreateStreetcodeHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        private readonly CreateStreetcodeHandler _handler;
        private readonly StreetcodeContent _streetcodeEntity;
        private readonly CreateStreetcodeDTO _streetcodeDTO;
        private readonly CreateStreetcodeCommand _command;

        public CreateStreetcodeHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _streetcodeEntity = new StreetcodeContent
            {
                Id = 1,
                Title = "Title",
                Alias = "alias",
                DateString = "string"
            };
            _streetcodeDTO = new CreateStreetcodeDTO
            {
                Title = "Title",
                Alias = "alias"
            };

            _command = new CreateStreetcodeCommand(_streetcodeDTO);
            _handler = new CreateStreetcodeHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenMapperReturnsDTO()
        {
            // Arrange
            _mockMapper
                .Setup(maper => maper.Map<StreetcodeContent>(_streetcodeDTO))
                .Returns(_streetcodeEntity);
            _mockMapper
                .Setup(maper => maper.Map<CreateStreetcodeDTO>(_streetcodeEntity))
                .Returns(_streetcodeDTO);
            _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.CreateAsync(_streetcodeEntity))
                .Callback<StreetcodeContent>(content =>
                {
                    content.Id = 1;
                }).ReturnsAsync(_streetcodeEntity);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsSuccess),
                () => Assert.Equal(_streetcodeDTO, result.Value));
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenMapperReturnsNull()
        {
            // Arrange
            _mockMapper
                .Setup(maper => maper.Map<StreetcodeContent>(_streetcodeDTO))
                .Returns((StreetcodeContent)null!);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);
            string expected = MessageResourceContext.GetMessage(ErrorMessages.FailToConvertNull, _command);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsFailed),
                () => Assert.Equal(expected, result.Errors.FirstOrDefault()?.Message));
        }

        [Fact]
        public async Task Handle_Should_ReturnError_WhenRepositorySaveChangesReturnsZero()
        {
            // Arrange
            _mockMapper
                .Setup(maper => maper.Map<StreetcodeContent>(_streetcodeDTO))
                .Returns(_streetcodeEntity);
            _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository.CreateAsync(_streetcodeEntity))
               .Callback<StreetcodeContent>(content =>
               {
                   content.Id = 1;
               }).ReturnsAsync(_streetcodeEntity);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);
            string expected = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateA, _command);

            // Assert
            Assert.Multiple(
               () => Assert.True(result.IsFailed),
               () => Assert.Equal(expected, result.Errors.FirstOrDefault()?.Message));
        }
    }
}
