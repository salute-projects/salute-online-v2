using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.DTO.Club;

namespace SaluteOnline.Domain.Conversion
{
    public static class ClubMappings
    {
        public static ClubDto ToDto(this Club club)
        {
            return new ClubDto
            {
                Country = club.Country,
                Id = club.Id,
                Registered = club.Registered,
                City = club.City,
                IsActive = club.IsActive,
                Status = club.Status,
                CreatorId = club.CreatorId,
                LastUpdate = club.LastUpdate,
                Logo = club.Logo,
                Description = club.Description,
                Title = club.Title,
                IsFiim = club.IsFiim
            };
        }
    }
}
