using System;

namespace SaluteOnline.API.DTO.User
{
    public class UserMainInfoDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Nickname { get; set; }
    }
}
