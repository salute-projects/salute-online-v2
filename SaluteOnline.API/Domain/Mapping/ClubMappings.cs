using System.Linq;
using Mapster;
using SaluteOnline.API.DTO.Club;
using SaluteOnline.API.DTO.User;

namespace SaluteOnline.API.Domain.Mapping
{
    public static class ClubMappings
    {
        public static ClubSummaryDto ToSummaryDto(this Club club, int currentUserId)
        {
            return new ClubSummaryDto
            {
                Country = club.Country,
                Id = club.Id,
                City = club.City,
                Logo = club.Logo,
                Description = club.Description,
                Title = club.Title,
                CanBeEdited = club.Administrators.Any(t => t.UserId == currentUserId)
            };
        }

        public static MembershipRequestDto ToDto(this MembershipRequest request)
        {
            return new MembershipRequestDto
            {
                Status = request.Status,
                Nickname = request.Nickname,
                Id = request.Id,
                LastActivity = request.LastActivity,
                SelectedFromExisting = request.SelectedFromExisting,
                Created = request.Created,
                UserInfo = request.User.Adapt<UserMainInfoDto>()
            };
        }
    }
}
