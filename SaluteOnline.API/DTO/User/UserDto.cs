﻿using System;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string SubjectId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string AlternativeEmail { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Vk { get; set; }
        public string Instagram { get; set; }
        public string Skype { get; set; }
        public string Nickname { get; set; }
        public Roles Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTimeOffset Registered { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public string Avatar { get; set; }
    }
}
