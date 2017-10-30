using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SaluteOnline.Domain.Common;
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
        public User Creator { get; set; }

        public ClubStatus Status { get; set; }
        public string Logo { get; set; }

    }
}
