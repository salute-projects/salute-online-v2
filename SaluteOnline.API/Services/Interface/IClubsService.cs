using System.Collections.Generic;
using System.Threading.Tasks;
using SaluteOnline.API.DTO.Club;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.Services.Interface
{
    public interface IClubsService
    {
        Task<int> CreateClub(CreateClubDto club, string subjectId);
        Page<ClubSummaryDto> GetClubs(ClubFilter filter, string subjectId);
        IEnumerable<ClubSummaryDto> GetMyClubs(string subjectId);
        ClubDto GetClub(int id, string subjectId);
        ClubInfoAggregation GetInfoAggregation();
        Page<ClubMemberSummary> GetClubAdministrators(ClubMembersFilter filter);
        Page<ClubMemberSummary> GetClubMembers(ClubMembersFilter filter);
        ClubMemberSummary AddClubMember(CreateClubMemberDto member, string subjectId);
        int AddMembershipRequest(MembershipRequestCreateDto request, string subjectId);
        Page<MembershipRequestDto> GetClubMembershipRequests(MembershipRequestFilter filter, string subjectId);
        void HandleMembershipRequest(HandleMembershipRequestDto dto, string subjectId);
        bool CanRegisterClub(string subjectId);

        #region Administration

        Page<ClubAdministrationSummaryDto> GetClubsForAdministration(ClubFilter filter);
        void ChangeClubStatus(ClubChangeStatusRequest request);

        #endregion
    }
}