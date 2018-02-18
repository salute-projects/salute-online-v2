using System.ComponentModel.DataAnnotations;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.Club
{
    public class ClubMembersFilter : BaseFilter
    {
        public string Search { get; set; }
        [Required]
        public int ClubId { get; set; }
    }
}
