using System;
using SaluteOnline.Domain.Common;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.Domain.Domain.EF
{
    public class MembershipRequest : IEntity
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public string Nickname { get; set; }
        public bool SelectedFromExisting { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public MembershipRequestStatus Status { get; set; }
        public int? ClubId { get; set; }
        public Club Club { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
