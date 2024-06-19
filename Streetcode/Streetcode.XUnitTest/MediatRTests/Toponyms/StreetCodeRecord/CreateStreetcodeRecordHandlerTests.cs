﻿using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Create;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Entity = Streetcode.DAL.Entities.Toponyms.StreetcodeToponym;
using EntityDTO = Streetcode.BLL.DTO.Toponyms.StreetcodeRecordDTO;

namespace Streetcode.XUnitTest.MediatRTests.Toponyms.StreetCodeRecord
{
    public class CreateStreetcodeRecordHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateStreetcodeRecordHandler _handler;
        private readonly Entity _recordEntity;
        private readonly EntityDTO _recordDTO;

        public CreateStreetcodeRecordHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateStreetcodeRecordHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);

            _recordEntity = new Entity()
            {
                StreetcodeId = 1, 
                ToponymId = 1,
                Streetcode = new StreetcodeContent(),
                Toponym = new Toponym(),
            };

            _recordDTO = new EntityDTO()
            {
                StreetcodeId = 1,
                ToponymId = 1,
                Streetcode = new StreetcodeDTO(),
                Toponym = new ToponymDTO(),
            };
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenRecordIsNotNull()
        {
            _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<EntityDTO>())).Returns(_recordEntity);
            _mockMapper.Setup(m => m.Map<EntityDTO>(It.IsAny<Entity>())).Returns(_recordDTO);
            _mockRepositoryWrapper.Setup(r => r.StreetcodeToponymRepository.CreateAsync(_recordEntity)).ReturnsAsync(_recordEntity);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            CreateStreetcodeRecordCommand request = new CreateStreetcodeRecordCommand(_recordDTO);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(_recordDTO, result.Value);
        }

        [Fact]
        public async Task Handle_Should_ReturnFail_WhenRecordIsNull()
        {
            _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<EntityDTO>())).Returns((Entity)null);
            _mockMapper.Setup(m => m.Map<EntityDTO>(It.IsAny<Entity>())).Returns((EntityDTO)null);
            _mockRepositoryWrapper.Setup(r => r.StreetcodeToponymRepository.CreateAsync(_recordEntity)).ReturnsAsync(_recordEntity);
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            CreateStreetcodeRecordCommand request = new CreateStreetcodeRecordCommand(_recordDTO);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.Errors.Any());
        }
    }
}