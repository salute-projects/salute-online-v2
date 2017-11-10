using System.ComponentModel.DataAnnotations;

namespace SaluteOnline.Domain.DTO.Club
{
    public class ClubMembersFilter : BaseFilter
    {
        public string Search { get; set; }
        [Required]
        public int ClubId { get; set; }
    }
}
