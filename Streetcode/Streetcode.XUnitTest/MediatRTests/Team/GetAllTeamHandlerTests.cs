using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetAll;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team
{
    /// <summary>
    /// Testing Team GetAll.
    /// </summary>
    public class GetAllTeamHandlerTests
    {
        private const string ErrorMsg = "Cannot find any Team";

        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<IRepositoryWrapper> mockRepositoryWrapper;
        private readonly Mock<ILoggerService> mockLogger;
        private readonly GetAllTeamHandler handler;

        private readonly List<TeamMemberDTO> membersDTO =
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

        private readonly List<TeamMember> members =
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
        /// Initializes a new instance of the <see cref="GetAllTeamHandlerTests"/> class.
        /// </summary>
        public GetAllTeamHandlerTests()
        {
            mockMapper = new Mock<IMapper>();
            mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            mockLogger = new Mock<ILoggerService>();
            handler = new GetAllTeamHandler(
                mockRepositoryWrapper.Object, mockMapper.Object, mockLogger.Object);
        }

        /// <summary>
        /// Checks Handle to success if team contains team_members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenTeamIsNotNull()
        {
            // Arrange
            ArrangeMockWrapper(members);
            mockMapper
            .Setup(m => m.Map<IEnumerable<TeamMemberDTO>>(members))
            .Returns(membersDTO);
            var query = new GetAllTeamQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        /// <summary>
        /// Checks Handle to return all members if team contains members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnAllTeams_WhenTeamIsNotNull()
        {
            // Arrange
            ArrangeMockWrapper(members);
            mockMapper
            .Setup(m => m.Map<IEnumerable<TeamMemberDTO>>(members))
            .Returns(membersDTO);
            var query = new GetAllTeamQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(membersDTO, result.Value);
        }

        /// <summary>
        /// Checks Handle to fail if no teams.
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenTeamIsNull()
        {
            // Arrange
            ArrangeMockWrapper();
            var query = new GetAllTeamQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        /// <summary>
        /// Checks Handle to error if no teams.
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_ReturnErrors_WhenTeamIsNull()
        {
            // Arrange
            ArrangeMockWrapper();
            var query = new GetAllTeamQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Errors);
        }

        /// <summary>
        /// Checks Handle to error "Cannot find any team" if no teams.
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_Should_SpecificError_WhenTeamIsNull()
        {
            // Arrange
            ArrangeMockWrapper();
            var query = new GetAllTeamQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            mockLogger.Verify(l => l.LogError(query, ErrorMsg), Times.Once);
            Assert.Equal(ErrorMsg, result.Reasons[0].Message);
        }

        private void ArrangeMockWrapper(List<TeamMember> memb = null!)
        {
            mockRepositoryWrapper
            .Setup(r => r.TeamRepository.GetAllAsync(
                It.IsAny<Expression<Func<TeamMember, bool>>>(),
                It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(memb);
        }
    }
}
