using System.Threading.Tasks;
using SaluteOnline.Domain.DTO.Auth0;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Providers.Interface
{
    public interface IAuthZeroProvider
    {
        Task<SignUpResponse> SignUp(UserEssential user);

        Task<LoginResponse> GetToken(UserEssential user);
    }
}