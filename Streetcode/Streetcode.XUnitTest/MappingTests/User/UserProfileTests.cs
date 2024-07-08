using AutoMapper;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Mapping.Users;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MappingTests.UserTests
{
    public class UserProfileTests
    {
        IMapper _mapper;

        public UserProfileTests()
        {
            var mapConf = new MapperConfiguration(conf => conf.AddProfile<UserProfile>());

            _mapper = mapConf.CreateMapper();
        }

        [Fact]
        public void SuccessMapping_WhenUserRegisterDtoMapsToUser()
        {
            // Arrange
            UserRegisterDTO userRegisterDTO = new UserRegisterDTO()
            {
                FirstName = "test",
                LastName = "test",
                Birthday = new DateOnly(1998, 12, 1),
                Username = "test",                
                Phone = "somephone",
                Email = "someEmail"
            };

            // Act
            User u = _mapper.Map<User>(userRegisterDTO);

            // Assert
            Assert.Multiple(
                () => Assert.IsType<User>(u),
                () => Assert.Equal(userRegisterDTO.FirstName, u.FirstName),
                () => Assert.Equal(userRegisterDTO.LastName, u.LastName),
                () => Assert.Equal(userRegisterDTO.Birthday, u.BirthDate),
                () => Assert.Equal(userRegisterDTO.Username, u.UserName),
                () => Assert.Equal(userRegisterDTO.Phone, u.PhoneNumber),
                () => Assert.Equal(userRegisterDTO.Email, u.Email));
        }

        [Fact]
        public void SuccessMapping_WhenUserDtoMapsToUser()
        {
            // Arrange
            UserDTO userDto = new UserDTO()
            {
                FirstName = "test",
                LastName = "test",
                Birthday = new DateOnly(1998, 12, 1),
                Username = "test",
                Phone = "somephone",
                Email = "someEmail",
                PhoneConfirmed = true,
                EmailConfirmed = true,
                TwoFactorEnabled = true,
            };

            // Act
            User u = _mapper.Map<User>(userDto);

            // Assert
            Assert.Multiple(
                () => Assert.IsType<User>(u),
                () => Assert.Equal(userDto.FirstName, u.FirstName),
                () => Assert.Equal(userDto.LastName, u.LastName),
                () => Assert.Equal(userDto.Birthday, u.BirthDate),
                () => Assert.Equal(userDto.Username, u.UserName),
                () => Assert.Equal(userDto.Phone, u.PhoneNumber),
                () => Assert.Equal(userDto.Email, u.Email),
                () => Assert.Equal(userDto.EmailConfirmed, u.EmailConfirmed),
                () => Assert.Equal(userDto.PhoneConfirmed, u.PhoneNumberConfirmed),
                () => Assert.Equal(userDto.TwoFactorEnabled, u.TwoFactorEnabled));
        }

        [Fact]
        public void SuccessMapping_WhenUserMapedToDeleteUserResponceDto()
        {
            // Arrange
            User u = new User()
            { 
                FirstName = "Test", 
                LastName = "Test"
            };

            // Act
            var deleteResponce = _mapper.Map<DeleteUserResponseDto>(u);

            // Assert
            Assert.Multiple(
                () => Assert.IsType<DeleteUserResponseDto>(deleteResponce),
                () => Assert.Equal(deleteResponce.Lastname, u.LastName),
                () => Assert.Equal(deleteResponce.Firstname, u.FirstName)
                );
        }
    }
}
