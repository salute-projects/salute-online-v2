using System.Threading.Tasks;
using SaluteOnline.Domain.DTO.Club;

namespace SaluteOnline.API.Services.Interface
{
    public interface IClubsService
    {
        Task<int> CreateClub(CreateClubDto club, string email);
    }
}