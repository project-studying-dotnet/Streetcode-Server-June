using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners
{
	public class DeletePartnerHandlerTests
	{
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private Partner _partner = new Partner { Id = 15 };
        private Partner _nullPartner;

        public DeletePartnerHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenPartnerIsNotNull()
        {
            // Arrange
            var query = new DeletePartnerCommand(15);
            var handler = new DeletePartnerHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
            _wrapperMock.Setup(obj => obj.PartnersRepository.GetFirstOrDefaultAsync(
                                                                     It.IsAny<Expression<Func<Partner, bool>>>(), default))
                                                                     .ReturnsAsync(_partner);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenPartnerIsNull()
        {
            // Arrange
            var query = new DeletePartnerCommand(15);
            var handler = new DeletePartnerHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
            _wrapperMock.Setup(obj => obj.PartnersRepository.GetFirstOrDefaultAsync(
                                                                     It.IsAny<Expression<Func<Partner, bool>>>(), default))
                                                                     .ReturnsAsync(_nullPartner);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }

}