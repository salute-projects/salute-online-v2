using System;
using System.ComponentModel.DataAnnotations;
using SaluteOnline.Domain.Common;
using Role = SaluteOnline.Domain.DTO.Enums.Role;

namespace SaluteOnline.API.DAL.Entities
{
    public class User : IEntity
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }

        [StringLength(25)]
        public string Auth0Id { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string AlternativeEmail { get; set; }
        [StringLength(50)]
        public string Facebook { get; set; }
        [StringLength(50)]
        public string Twitter { get; set; }
        [StringLength(50)]
        public string Vk { get; set; }
        [StringLength(50)]
        public string Instagram { get; set; }
        [StringLength(50)]
        public string Skype { get; set; }

        public bool IsActive { get; set; }
        public Role Role { get; set; }

        public DateTimeOffset Registered { get; set; }
        public DateTimeOffset LastActivity { get; set; }
    }
}
