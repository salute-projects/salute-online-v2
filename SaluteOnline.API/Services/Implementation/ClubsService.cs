using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
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
    }
}
