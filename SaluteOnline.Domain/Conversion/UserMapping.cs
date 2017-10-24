using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.Domain.Conversion
{
    public static class UserMapping
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LastActivity = user.LastActivity,
                Registered = user.Registered,
                Role = user.Role,
                Address = user.Address,
                AlternativeEmail = user.AlternativeEmail,
                City = user.City,
                Country = user.Country,
                DateOfBirth = user.DateOfBirth,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                IsActive = user.IsActive,
                Phone = user.Phone,
                Skype = user.Skype,
                Twitter = user.Twitter,
                Vk = user.Vk
            };
        }

        public static User FromDto(this UserDto userDto)
        {
            return new User
            {
                Email = userDto.Email,
                Address = userDto.Address,
                AlternativeEmail = userDto.AlternativeEmail,
                City = userDto.City,
                Country = userDto.Country,
                DateOfBirth = userDto.DateOfBirth,
                Facebook = userDto.Facebook,
                FirstName = userDto.FirstName,
                Instagram = userDto.Instagram,
                IsActive = userDto.IsActive,
                Id = userDto.Id,
                LastActivity = userDto.LastActivity,
                LastName = userDto.LastName,
                Phone = userDto.Phone,
                Registered = userDto.Registered,
                Role = userDto.Role,
                Skype = userDto.Skype,
                Twitter = userDto.Twitter,
                Vk = userDto.Vk
            };
        }
    }
}
