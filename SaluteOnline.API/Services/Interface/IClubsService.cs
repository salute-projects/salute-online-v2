using System.Collections.Generic;
using System.Threading.Tasks;
using SaluteOnline.API.DTO.Club;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.Services.Interface
{
    public interface IClubsService
    {
        Task<int> CreateClub(CreateClubDto club, string email);
        Page<ClubSummaryDto> GetClubs(ClubFilter filter, string email);
        IEnumerable<ClubSummaryDto> GetMyClubs(string email);
        ClubDto GetClub(int id, string email);
        ClubInfoAggregation GetInfoAggregation();
        Page<ClubMemberSummary> GetClubAdministrators(ClubMembersFilter filter);
        Page<ClubMemberSummary> GetClubMembers(ClubMembersFilter filter);
        ClubMemberSummary AddClubMember(CreateClubMemberDto member, string email);
        int AddMembershipRequest(MembershipRequestCreateDto request, string email);
        Page<MembershipRequestDto> GetClubMembershipRequests(MembershipRequestFilter filter, string email);
        void HandleMembershipRequest(HandleMembershipRequestDto dto, string email);
    }
}