using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.Domain
{
    public class Player : IEntity
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }

        public string Nickname { get; set; }
        public DateTimeOffset Registered { get; set; }
        public DateTimeOffset LastChanged { get; set; }

        public bool IsActive { get; set; }
        public string Avatar { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        [ForeignKey("Club")]
        public int ClubId { get; set; }
        [JsonIgnore]
        public Club Club { get; set; }
    }
}
