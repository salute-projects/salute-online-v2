using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.EF.LinkEntities;
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
                var relation = new ClubUserAdministrator
                {
                    User = user,
                    Club = newClub
                };
                newClub.Administrators.Add(relation);
                user.ClubsAdministrated.Add(relation);
                _unitOfWork.Save();
                _activityService.LogActivity(new Activity
                {
                    UserId = user.Id,
                    Importance = ActivityImportance.High,
                    Type = ActivityType.NewClubAdded,
                    Data = JsonConvert.SerializeObject(newClub, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
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

        public Page<ClubDto> GetClubs(ClubFilter filter, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new ArgumentException("Internal error happened. Please try a bit later");
                Func<Club, bool> searchCriteria =
                    t => (!filter.IsActive.HasValue || t.IsActive == filter.IsActive.Value)
                         && (!filter.IsFiim.HasValue || t.IsFiim == filter.IsFiim.Value)
                         && (string.IsNullOrEmpty(filter.Country) || string.Equals(t.Country, filter.Country, StringComparison.CurrentCultureIgnoreCase))
                         && (string.IsNullOrEmpty(filter.City) || string.Equals(t.City, filter.City, StringComparison.CurrentCultureIgnoreCase))
                         && (string.IsNullOrEmpty(filter.Title) || t.Title.ToLower().StartsWith(filter.Title.ToLower()))
                         && (!filter.CreatorId.HasValue || t.CreatorId == filter.CreatorId.Value)
                         && (filter.Status == ClubStatus.ActiveAndPending ? (t.Status == ClubStatus.Active || t.Status == ClubStatus.PendingActivation) : t.Status == filter.Status);
                var skip = filter.Page == 0 ? 0 : (filter.Page - 1) * (filter.PageSize ?? 25);
                var take = filter.PageSize ?? 25;
                var orderByField = string.IsNullOrEmpty(filter.OrderBy) ? nameof(Club.LastUpdate) : filter.OrderBy;
                var allClubs =
                    _unitOfWork.Clubs.GetAsQueryable(t => searchCriteria(t))
                        .Include(t => t.Administrators)
                        .ThenInclude(t => t.User)
                        .Include(t => t.Players)
                        .ThenInclude(t => t.User);
                var slice = (filter.Asc ? 
                    allClubs.OrderBy(t => typeof(Club).GetProperty(orderByField).GetValue(t)) : 
                    allClubs.OrderByDescending(t => typeof(Club).GetProperty(orderByField).GetValue(t)))
                    .Skip(skip)
                    .Take(take);
                var allCount = allClubs.Select(t => t.Id).Count();
                var page = new Page<ClubDto>(filter.Page, filter.PageSize ?? 25, allCount, slice.Select(t => t.ToDto(currentUser.Id)));
                return page;
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

        public ClubInfoAggregation GetInfoAggregation()
        {
            try
            {
                var allClubs = _unitOfWork.Clubs.GetAsQueryable();
                var geography =
                    allClubs.Select(t => t.Country)
                        .Distinct()
                        .Select(t => new KeyValuePair<string, IEnumerable<string>>(t,
                            allClubs.Where(x => x.Country == t).Select(q => q.City)));
                var byStatus =
                    allClubs.Select(t => t.Status).Distinct().Select(
                            t => new KeyValuePair<ClubStatus, int>(t, allClubs.Count(r => r.Status == t)));
                var result = new ClubInfoAggregation
                {
                    Count = allClubs.Count(),
                    IsFiim = allClubs.Count(t => t.IsFiim),
                    ByStatus = byStatus,
                    Geography = geography
                };
                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading club info aggregation. Please try a bit later");
            }
        }
    }
}
