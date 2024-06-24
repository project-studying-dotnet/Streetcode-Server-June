using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Mapping.Streetcode;
using Streetcode.BLL.Util;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.MappingTests.StreetcodeBlock
{
    public class StreetcodeProfileTests
    {
        private readonly IMapper _mapper;

        public StreetcodeProfileTests()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new StreetcodeProfile());
            });

            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public void Profile_Should_MapsCorrectly_WhenEventStreetcode()
        {
            var dto = new CreateStreetcodeDTO
            {
                StreetcodeType = StreetcodeType.Event,
            };

            var mapped = _mapper.Map<StreetcodeContent>(dto);

            Assert.IsType<EventStreetcode>(mapped);
        }

        [Fact]
        public void Profile_Should_MapsCorrectly_WhenPersonStreetcode()
        {
            var dto = new CreateStreetcodeDTO
            {
                StreetcodeType = StreetcodeType.Person,
                FirstName = "John",
                LastName = "Doe",
                Rank = "Captain"
            };

            var mapped = _mapper.Map<StreetcodeContent>(dto);

            Assert.IsType<PersonStreetcode>(mapped);
            Assert.Equal(dto.FirstName, ((PersonStreetcode)mapped).FirstName);
            Assert.Equal(dto.LastName, ((PersonStreetcode)mapped).LastName);
            Assert.Equal(dto.Rank, ((PersonStreetcode)mapped).Rank);
        }

        [Fact]
        public void Profile_Should_MapsCorrectly_WhenDateString()
        {
            var dto = new CreateStreetcodeDTO
            {
                EventStartOrPersonBirthDate = DateTime.UtcNow,
                EventEndOrPersonDeathDate = DateTime.UtcNow.AddDays(1)
            };
            string expected = DateToStringConverter.CreateDateString(
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

            var mapped = _mapper.Map<StreetcodeContent>(dto);

            Assert.Equal(expected, mapped.DateString);
        }
    }
}
