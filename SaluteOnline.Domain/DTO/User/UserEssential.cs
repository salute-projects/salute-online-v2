using System.ComponentModel.DataAnnotations;

namespace SaluteOnline.Domain.DTO.User
{
    public class UserEssential
    {
        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }
    }
}
