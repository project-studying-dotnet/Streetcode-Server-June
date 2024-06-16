namespace Streetcode.XUnitTest.MediatRTests.Team
{
    using System.Linq.Expressions;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Streetcode.BLL.DTO.Team;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Team.GetById;
    using Streetcode.DAL.Entities.Team;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;

    /// <summary>
    /// Testing Team GetAll.
    /// </summary>
    public class GetByIdTeamTests
    {
        private const string Errormsg = "Cannot find any team with corresponding id: ";

        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetByIdTeamHandler _handler;
        private readonly List<TeamMemberDTO> _membersDto =
            new()
            {
                new TeamMemberDTO
                {
                    Id = 1, FirstName = "Test", LastName = "Test_Last",
                    Description = "Test_desc", IsMain = true,
                    ImageId = 1,
                },
                new TeamMemberDTO
                {
                    Id = 2, FirstName = "Test", LastName = "Test_Last",
                    Description = "Test_desc", IsMain = false,
                    ImageId = 2,
                },
            };

        private readonly List<TeamMember> _members =
            new()
            {
                new TeamMember
                {
                    Id = 1, FirstName = "Test", LastName = "Test_last",
                    Description = "Test_desc", IsMain = true,
                    ImageId = 1,
                },
                new TeamMember
                {
                    Id = 2, FirstName = "Test", LastName = "Test_last",
                    Description = "Test_desc", IsMain = false,
                    ImageId = 2,
                },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="GetByIdTeamTests"/> class.
        /// </summary>
        public GetByIdTeamTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new GetByIdTeamHandler(
                _mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Checks Handle to success if team contains team_member with correct id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenTeamIsNotNull()
        {
            // Arrange
            int id = 1;
            var memberResult = _membersDto.SingleOrDefault(m => m.Id == id);
            ArrangeMockWrapper(id);
            _mockMapper
            .Setup(m => m.Map<TeamMemberDTO>(_members[0]))
            .Returns(memberResult!);
            var query = new GetByIdTeamQuery(id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        /// <summary>
        /// Checks Handle to return correct team_member with correct id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnTeam_WhenCorrectId()
        {
            // Arrange
            int id = 1;
            var memberResult = _membersDto.SingleOrDefault(m => m.Id == id);
            ArrangeMockWrapper(id);

            _mockMapper
            .Setup(m => m.Map<TeamMemberDTO>(_members[0]))
            .Returns(memberResult!);
            var query = new GetByIdTeamQuery(id);

            // Act  
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(memberResult, result.Value);
        }

        /// <summary>
        /// Checks Handle to fail if id don`t exist.
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenNonExistingId()
        {
            // Arrange
            int id = -1;
            ArrangeMockWrapper();
            var query = new GetByIdTeamQuery(id);
            var memberResult = _membersDto.SingleOrDefault(m => m.Id == id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        /// <summary>
        /// Checks Handle to error if id don`t exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnErrors_WhenNonExistingId()
        {
            // Arrange
            int id = -1;
            ArrangeMockWrapper();
            var query = new GetByIdTeamQuery(id);
            var memberResult = _membersDto.SingleOrDefault(m => m.Id == id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Errors);
        }

        /// <summary>
        /// Checks Handle to error "Cannot find any team with corresponding id:" if id don`t exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_SpecificError_WhenNonExistingId()
        {
            // Arrange
            int id = -1;
            ArrangeMockWrapper();
            var query = new GetByIdTeamQuery(id);
            var memberResult = _membersDto.SingleOrDefault(m => m.Id == id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogError(query, $"{Errormsg}{id}"), Times.Once);
            Assert.Contains(result.Reasons, m => m.Message == $"{Errormsg}{id}");
        }

        private void ArrangeMockWrapper(int? id = null)
        {
            if (id != null)
            {
                _mockRepositoryWrapper
            .Setup(r => r.TeamRepository.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(_members[(int)id - 1]);
            }
            else
            {
                _mockRepositoryWrapper
            .Setup(r => r.TeamRepository.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((TeamMember)null!);
            }
        }
    }
}
