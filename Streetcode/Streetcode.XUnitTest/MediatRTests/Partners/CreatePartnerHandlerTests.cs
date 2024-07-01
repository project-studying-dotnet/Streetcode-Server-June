using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners
{
    public class CreatePartnerHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private Partner _nullPartner;
        private PartnerDTO _partnerDTO = new PartnerDTO() { Id = 2 };
        private Partner _partner = new () { Id = 2 };
        private List<StreetcodeContent> _streetcodes = new()
        {
            new StreetcodeContent()
            {
                Id = 2,
                Title = "string",
            },
            new StreetcodeContent()
            {
                Id = 3,
                Title = "string",
            },
        };

        private CreatePartnerDTO _createPartnerDTO = new ()
        {
            Id = 2,
            Streetcodes = new List<StreetcodeShortDTO>()
            {
                new StreetcodeShortDTO()
                {
                    Id = 2,
                    Title = "string",
                },
                new StreetcodeShortDTO()
                {
                    Id = 3,
                    Title = "string",
                },
            },
        };

        public CreatePartnerHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnEqualTrue_WhenInputNewPartner()
        {
            // Arrange
            _mapperMock.Setup(obj => obj.Map<PartnerDTO>(It.IsAny<object>())).Returns(_partnerDTO);
            _mapperMock.Setup(obj => obj.Map<Partner>(It.IsAny<object>())).Returns(_partner);
            _wrapperMock.Setup(obj => obj.PartnersRepository.CreateAsync(_partner)).ReturnsAsync(_partner);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetAllAsync(default, default)).ReturnsAsync(_streetcodes);

            CreatePartnerCommand request = new CreatePartnerCommand(_createPartnerDTO);
            CreatePartnerHandler handler = new CreatePartnerHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Result.Value.Id);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailed_WhenInputEmptyPartner()
        {
            // Arrange
            _mapperMock.Setup(obj => obj.Map<PartnerDTO>(It.IsAny<object>())).Returns(new PartnerDTO());
            _mapperMock.Setup(obj => obj.Map<Partner>(It.IsAny<object>())).Returns(_nullPartner);
            _wrapperMock.Setup(obj => obj.PartnersRepository.CreateAsync(_partner)).ReturnsAsync(_nullPartner);
            _wrapperMock.Setup(obj => obj.StreetcodeRepository.GetAllAsync(default, default)).ReturnsAsync(new List<StreetcodeContent>());


            CreatePartnerCommand request = new CreatePartnerCommand(_createPartnerDTO);
            CreatePartnerHandler handler = new CreatePartnerHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Result.IsFailed);
        }
    }
}
