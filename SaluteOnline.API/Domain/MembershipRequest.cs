using System;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.Domain
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
