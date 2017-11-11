using System.Threading.Tasks;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Club;

namespace SaluteOnline.API.Services.Interface
{
    public interface IClubsService
    {
        Task<int> CreateClub(CreateClubDto club, string email);
        Page<ClubSummaryDto> GetClubs(ClubFilter filter, string email);
        ClubDto GetClub(int id, string email);
        ClubInfoAggregation GetInfoAggregation();
        Page<ClubMemberSummary> GetClubAdministrators(ClubMembersFilter filter);
        Page<ClubMemberSummary> GetClubMembers(ClubMembersFilter filter);
        ClubMemberSummary AddClubMember(CreateClubMemberDto member, string email);
        int AddMembershipRequest(MembershipRequestCreateDto request, string email);
        Page<MembershipRequestDto> GetClubMembershipRequests(EntityFilter filter, string email);
    }
}