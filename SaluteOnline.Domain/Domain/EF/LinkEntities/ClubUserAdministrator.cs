namespace SaluteOnline.Domain.Domain.EF.LinkEntities
{
    public class ClubUserAdministrator
    {
        public int ClubId { get; set; }
        public Club Club { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
