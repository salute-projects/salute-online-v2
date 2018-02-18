using System;
using SaluteOnline.API.DTO.User;

namespace SaluteOnline.API.Domain.Mapping
{
    public static class UserMapping
    {
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
    }
}
