using System;
using System.Collections.Generic;
using System.Text;

namespace SaluteOnline.Domain.Domain.EF.LinkEntities
{
    public class ClubUserPlayers
    {
        public int ClubId { get; set; }
        public Club Club { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Nickname { get; set; }
    }
}
