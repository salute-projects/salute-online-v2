using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SaluteOnline.Domain.Common;
using SaluteOnline.Domain.Domain.EF.LinkEntities;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.Domain.Domain.EF
{
    public class Club : IEntity
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        public string Description { get; set; }

        public DateTimeOffset Registered { get; set; }
        public DateTimeOffset LastUpdate { get; set; }

        public bool IsFiim { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(Creator))]
        public int CreatorId { get; set; }
        [JsonIgnore]
        public User Creator { get; set; }

        public ClubStatus Status { get; set; }
        public string Logo { get; set; }

        [JsonIgnore]
        public ICollection<ClubUserAdministrator> Administrators { get; set; } = new List<ClubUserAdministrator>();

        [JsonIgnore]
        public ICollection<ClubUserPlayers> Players { get; set; } = new List<ClubUserPlayers>();
    }
}
