using System;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.DTO.Club;
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
                Vk = user.Vk,
                Nickname = user.Nickname,
                Avatar = user.Avatar
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
                Vk = userDto.Vk,
                Nickname = userDto.Nickname,
                Avatar = userDto.Avatar
            };
        }

        public static void UpdateEntity(this UserDto userDto, User user)
        {
            user.Address = userDto.Address;
            user.AlternativeEmail = userDto.AlternativeEmail;
            user.City = userDto.City;
            user.Country = userDto.Country;
            user.DateOfBirth = userDto.DateOfBirth;
            user.Facebook = userDto.Facebook;
            user.FirstName = userDto.FirstName;
            user.Instagram = userDto.Instagram;
            user.LastName = userDto.LastName;
            user.Phone = userDto.Phone;
            user.Skype = userDto.Skype;
            user.Twitter = userDto.Twitter;
            user.Vk = userDto.Vk;
            user.Nickname = userDto.Nickname;
            user.LastActivity = DateTimeOffset.UtcNow;

        }

        public static void UpdateEntity(this UserMainInfoDto userDto, User user)
        {
            user.Facebook = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Nickname = userDto.Nickname;
            user.DateOfBirth = userDto.DateOfBirth;
            user.LastActivity = DateTimeOffset.UtcNow;
        }

        public static void UpdateEntity(this UserPersonalInfoDto userDto, User user)
        {
            user.Address = userDto.Address;
            user.AlternativeEmail = userDto.AlternativeEmail;
            user.City = userDto.City;
            user.Country = userDto.Country;
            user.Facebook = userDto.Facebook;
            user.Twitter = userDto.Twitter;
            user.Instagram = userDto.Instagram;
            user.Vk = userDto.Vk;
            user.Skype = userDto.Skype;
            user.Phone = userDto.Phone;
            user.LastActivity = DateTimeOffset.UtcNow;
        }

        public static UserMainInfoDto ToMainInfoDto(this User user)
        {
            return new UserMainInfoDto
            {
                Id = user.Id,
                Nickname = user.Nickname,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };
        }
    }
}
