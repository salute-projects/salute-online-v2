namespace SaluteOnline.Domain.DTO.User
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
