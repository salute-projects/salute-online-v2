using System.Threading.Tasks;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO.Club;

namespace SaluteOnline.API.Services.Interface
{
    public interface IClubsService
    {
        Task<int> CreateClub(CreateClubDto club, string email);
        Page<ClubDto> GetClubs(ClubFilter filter);
    }
}