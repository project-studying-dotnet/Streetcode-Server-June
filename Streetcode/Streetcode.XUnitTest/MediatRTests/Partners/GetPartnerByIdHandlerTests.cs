using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners
{
    public class GetPartnerByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private PartnerDTO _partnerDto = new PartnerDTO() { Id = 2 };
        private Partner _partner = new Partner() { Id = 2 };

        public GetPartnerByIdHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnEqualTrue_WhenPartnerEqualToExpected()
        {
            // Arrange
            var expectedId = 2;
            _mapperMock.Setup(obj => obj.Map<PartnerDTO>(It.IsAny<object>())).Returns(_partnerDto);
            _wrapperMock.Setup(obj => obj.PartnersRepository
                                              .GetSingleOrDefaultAsync(
                                              It.IsAny<Expression<Func<Partner, bool>>>(),
                                              It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
                                              .ReturnsAsync(_partner);
            var handler = new GetPartnerByIdHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            Result<PartnerDTO> result = await handler.Handle(new GetPartnerByIdQuery(expectedId), CancellationToken.None);

            // Assert
            Assert.Equal(expectedId, result.Value.Id);
        }

        [Fact]
        public async Task Handle_Should_ReturnEqualFalse_WhenPartnerNotEqualToExpected()
        {
            // Arrange
            var expectedId = 3;
            _mapperMock.Setup(obj => obj.Map<PartnerDTO>(It.IsAny<object>())).Returns(new PartnerDTO());
            _wrapperMock.Setup(obj => obj.PartnersRepository
                                              .GetSingleOrDefaultAsync(
                                              It.IsAny<Expression<Func<Partner, bool>>>(),
                                              It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
                                              .ReturnsAsync(new Partner());
            var handler = new GetPartnerByIdHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            Result<PartnerDTO> result = await handler.Handle(new GetPartnerByIdQuery(expectedId), CancellationToken.None);

            // Assert
            Assert.NotEqual(expectedId, result.Value.Id);
        }
    }
}
