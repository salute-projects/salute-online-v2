using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.Mongo;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Club;

namespace SaluteOnline.API.Services.Implementation
{
    public class ClubsService : IClubsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IActivityService _activityService;

        public ClubsService(IUnitOfWork unitOfWork, ILogger<ClubsService> logger, IActivityService activityService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityService = activityService;
        }

        public async Task<int> CreateClub(CreateClubDto club, string email)
        {
            try
            {
                var user = (await _unitOfWork.Users.GetAsync(t => t.Email == email)).SingleOrDefault();
                if (user == null)
                    throw new ArgumentException("User not found");
                var newClub = new Club
                {
                    Guid = Guid.NewGuid(),
                    IsActive = false,
                    Status = ClubStatus.PendingActivation,
                    Description = club.Description,
                    Title = club.Title,
                    Country = club.Country,
                    City = club.City,
                    Creator = user,
                    Registered = DateTimeOffset.UtcNow,
                    LastUpdate = DateTimeOffset.UtcNow
                };
                await _unitOfWork.Clubs.InsertAsync(newClub);
                _unitOfWork.Save();
                _activityService.LogActivity(new Activity
                {
                    UserId = user.Id,
                    Importance = ActivityImportance.High,
                    Type = ActivityType.NewClubAdded,
                    Data = JsonConvert.SerializeObject(newClub)
                });
                return newClub.Id;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while adding new club. Please try a bit later");
            }
        }

        public async Task<Page<ClubDto>> GetClubs(ClubFilter filter)
        {
            try
            {
                Func<Club, bool> searchCriteria =
                    t => (!filter.IsActive.HasValue || t.IsActive == filter.IsActive.Value)
                         && (!filter.IsFiim.HasValue || t.IsFiim == filter.IsFiim.Value);
                var clubs = await _unitOfWork.Clubs.GetPageAsync(filter.Page, filter.PageSize ?? 25, t => searchCriteria(t));
                return new Page<ClubDto>
                {
                    Items = clubs.Items.Select(t => t.ToDto()).ToList(),
                    Total = clubs.Total,
                    TotalPages = clubs.TotalPages,
                    PageSize = clubs.PageSize,
                    CurrentPage = clubs.CurrentPage
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while fetching list of clubs. Please try a bit later");
            }
        }
    }
}
