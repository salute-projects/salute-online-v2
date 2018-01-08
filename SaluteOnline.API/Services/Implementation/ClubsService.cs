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

        public Page<ClubSummaryDto> GetClubs(ClubFilter filter, string email)
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
                var page = new Page<ClubSummaryDto>(filter.Page, filter.PageSize ?? 25, allCount, slice.Select(t => t.ToSummaryDto(currentUser.Id)));
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

        public IEnumerable<ClubSummaryDto> GetMyClubs(string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new ArgumentException("Internal error happened. Please try a bit later");
                return
                    _unitOfWork.Users.GetAsQueryable()
                        .Include(t => t.ClubsAdministrated)
                        .ThenInclude(t => t.Club)
                        .SingleOrDefault(t => t.Id == currentUser.Id)
                        .ClubsAdministrated.Select(t => t.Club.ToSummaryDto(currentUser.Id));
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading clubs list. Please try a bit later");
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

        public ClubDto GetClub(int id, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new ArgumentException("Internal error happened. Please try a bit later");
                var club = _unitOfWork.Clubs.GetById(id: id);
                if (club == null)
                    throw new ArgumentException("Error while getting club info. Please try a bit later");
                if (currentUser.Role == Role.User && club.Administrators.All(t => t.UserId != id))
                    throw new ArgumentException("Operation not allowed");
                return club.ToDto(id);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading club info. Please try a bit later");
            }
        }

        public Page<ClubMemberSummary> GetClubAdministrators(ClubMembersFilter filter)
        {
            try
            {
                var club =
                    _unitOfWork.Clubs.GetAsQueryable()
                        .Include(t => t.Administrators)
                        .ThenInclude(t => t.User)
                        .FirstOrDefault(t => t.Id == filter.ClubId);
                if (club == null)
                    throw new ArgumentException("Club not found");
                var orderByField = string.IsNullOrEmpty(filter.OrderBy) ? nameof(ClubMemberSummary.Registered) : filter.OrderBy;
                var all = club.Administrators.Where(t => string.IsNullOrEmpty(filter.Search) || t.User != null && t.User.Nickname.ToLower().Contains(filter.Search.ToLower()))
                    .Select(t => new ClubMemberSummary
                {
                    Country = t.User.Country,
                    Email = t.User.Email,
                    Registered = t.Registered,
                    City = t.User.City,
                    UserId = t.UserId,
                    IsActive = t.IsActive,
                    Avatar = t.User.Avatar,
                    LastName = t.User.LastName,
                    FirstName = t.User.FirstName,
                    Nickname = t.User.Nickname
                }).ToList();
                all = filter.Asc
                    ? all.OrderBy(t => typeof(ClubMemberSummary).GetProperty(orderByField).GetValue(t)).ToList()
                    : all.OrderByDescending(t => typeof(ClubMemberSummary).GetProperty(orderByField).GetValue(t)).ToList();
                var slice = all.Skip((filter.Page - 1) * filter.PageSize ?? 25).Take(filter.PageSize ?? 25);
                return new Page<ClubMemberSummary>(filter.Page, filter.PageSize ?? 25, all.Count, slice);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading club administrators list. Please try a bit later");
            }
        }

        public Page<ClubMemberSummary> GetClubMembers(ClubMembersFilter filter)
        {
            try
            {
                var club =
                    _unitOfWork.Clubs.GetAsQueryable()
                        .Include(t => t.Players)
                        .ThenInclude(t => t.User)
                        .FirstOrDefault(t => t.Id == filter.ClubId);
                if (club == null)
                    throw new ArgumentException("Club not found");
                var orderByField = string.IsNullOrEmpty(filter.OrderBy) ? nameof(ClubMemberSummary.Registered) : filter.OrderBy;
                var all = club.Players.Where(t => string.IsNullOrEmpty(filter.Search) || t.Nickname.ToLower().Contains(filter.Search))
                    .Select(t => new ClubMemberSummary
                {
                    Country = t.User?.Country,
                    Email = t.User?.Email,
                    Registered = t.Registered,
                    City = t.User?.City,
                    UserId = t.UserId,
                    IsActive = t.IsActive,
                    Avatar = t.Avatar ?? t.User?.Avatar,
                    FirstName = t.User?.FirstName,
                    LastName = t.User?.LastName,
                    Nickname = t.Nickname ?? t.User?.Nickname,
                    PlayerId = t.Id
                }).ToList();
                all = filter.Asc
                    ? all.OrderBy(t => typeof(ClubMemberSummary).GetProperty(orderByField).GetValue(t)).ToList()
                    : all.OrderByDescending(t => typeof(ClubMemberSummary).GetProperty(orderByField).GetValue(t)).ToList();
                var slice = all.Skip((filter.Page - 1) * filter.PageSize ?? 25).Take(filter.PageSize ?? 25);
                return new Page<ClubMemberSummary>(filter.Page, filter.PageSize ?? 25, all.Count, slice);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading club members list. Please try a bit later");
            }
        }

        public ClubMemberSummary AddClubMember(CreateClubMemberDto member, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(member.Nickname))
                    throw new ArgumentException("Nickname cannot be empty");
                var club = _unitOfWork.Clubs.GetAsQueryable(y => y.Id == member.ClubId).Include(t => t.Administrators).Include(t => t.Players).FirstOrDefault();
                if (club == null)
                    throw new ArgumentException("Club not found");
                var currentMember = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentMember == null)
                    throw new ArgumentException("Error while authorization");
                if (club.Administrators.All(t => t.UserId != currentMember.Id))
                    throw new ArgumentException("Operation not allowed");
                var alreadyExists = club.Players.Any(t => string.Equals(t.Nickname, member.Nickname, StringComparison.CurrentCultureIgnoreCase));
                if (alreadyExists)
                    throw new ArgumentException("Member with that nickname already exists");
                var newPlayer = new Player
                {
                    ClubId = member.ClubId,
                    Guid = Guid.NewGuid(),
                    IsActive = true,
                    Registered = DateTimeOffset.UtcNow,
                    LastChanged = DateTimeOffset.UtcNow,
                    Nickname = member.Nickname
                };
                club.Players.Add(newPlayer);
                _unitOfWork.Save();
                return new ClubMemberSummary
                {
                    Nickname = member.Nickname,
                    IsActive = newPlayer.IsActive,
                    PlayerId = newPlayer.Id
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while adding club members. Please try a bit later");
            }
        }

        public int AddMembershipRequest(MembershipRequestCreateDto request, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Nickname))
                    throw new ArgumentException("Nickname cannot be empty");
                var existingMember = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (existingMember == null)
                    throw new ArgumentException("User not found");
                var club = _unitOfWork.Clubs.GetAsQueryable(t => t.Id == request.ClubId)
                    .Include(t => t.Administrators).ThenInclude(t => t.User)
                    .Include(t => t.MembershipRequests).SingleOrDefault();
                if (club == null)
                    throw new ArgumentException("Club not found");
                if (club.MembershipRequests.Any(t => t.Nickname == request.Nickname))
                    throw new ArgumentException($"Membership request for nickname {request.Nickname} already exists");
                var newRequest = new MembershipRequest
                {
                    ClubId = request.ClubId,
                    UserId = existingMember.Id,
                    Created = DateTimeOffset.UtcNow,
                    LastActivity = DateTimeOffset.UtcNow,
                    Nickname = request.Nickname,
                    SelectedFromExisting = request.SelectedFromExisting,
                    Status = MembershipRequestStatus.Pending
                };
                club.MembershipRequests.Add(newRequest);
                var messageGuid = Guid.NewGuid();
                foreach (var administrator in club.Administrators)
                {
                    administrator.User.InnerMessagesReceived.Add(new InnerMessage
                    {
                        Status = MessageStatus.Pending,
                        Body = $"You have a new membership request in club {club.Title} from user with nickname {request.Nickname}",
                        Created = DateTimeOffset.UtcNow,
                        Guid = messageGuid,
                        OneResponseForAll = true,
                        LastActivity = DateTimeOffset.UtcNow,
                        SentBySystem = true,
                        Title = "New Membership Request",
                        SenderType = EntityType.System,
                        ReceiverType = EntityType.User,
                        ReceiverId = administrator.UserId
                    });
                }
                _unitOfWork.Save();
                return newRequest.Id;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while adding membership request. Please try a bit later");
            }
        }

        public Page<MembershipRequestDto> GetClubMembershipRequests(MembershipRequestFilter filter, string email)
        {
            try
            {
                var existingMember = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (existingMember == null)
                    throw new ArgumentException("User not found");
                var club = _unitOfWork.Clubs.GetAsQueryable(t => t.Id == filter.EntityId).Include(t => t.MembershipRequests).ThenInclude(t => t.User).FirstOrDefault();
                if (club == null)
                    throw new ArgumentException("Club not found");
                var allRequests = string.IsNullOrEmpty(filter.SearchBy) ? 
                    club.MembershipRequests.Where(t => t.Status == filter.Status).ToList() : 
                    club.MembershipRequests.Where(t => t.Nickname.ToLower().Contains(filter.SearchBy.ToLower()) && t.Status == filter.Status).ToList();
                var skip = filter.Page == 0 ? 0 : (filter.Page - 1) * (filter.PageSize ?? 25);
                var take = filter.PageSize ?? 25;
                var orderByField = string.IsNullOrEmpty(filter.OrderBy) ? nameof(MembershipRequest.Created) : filter.OrderBy;
                var allCount = allRequests.Count;
                var slice = (filter.Asc ?
                    allRequests.OrderBy(t => typeof(MembershipRequest).GetProperty(orderByField).GetValue(t)) :
                    allRequests.OrderByDescending(t => typeof(MembershipRequest).GetProperty(orderByField).GetValue(t)))
                    .Skip(skip)
                    .Take(take);
                var page = new Page<MembershipRequestDto>(filter.Page, filter.PageSize ?? 25, allCount, slice.Select(t => t.ToDto()));
                return page;

            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while fetching club membership requests. Please try a bit later");
            }
        }

        public void HandleMembershipRequest(HandleMembershipRequestDto dto, string email)
        {
            try
            {
                var existingMember = _unitOfWork.Users.Get(t => t.Email == email).SingleOrDefault();
                if (existingMember == null)
                    throw new ArgumentException("User not found");
                var club =
                    _unitOfWork.Clubs.GetAsQueryable(t => t.Id == dto.ClubId)
                        .Include(t => t.Players)
                        .Include(t => t.MembershipRequests).ThenInclude(t => t.User)
                        .Include(t => t.Administrators)
                        .FirstOrDefault();
                if (club == null)
                    throw new ArgumentException("Club not found");
                if (club.Administrators.All(t => t.UserId != existingMember.Id))
                    throw new ArgumentException("Operation not allowed");
                var request = club.MembershipRequests.SingleOrDefault(t => t.Id == dto.RequestId);
                if (request == null)
                    throw new ArgumentException("Request not found");
                request.Status = dto.Status;
                request.LastActivity = DateTimeOffset.UtcNow;
                if (dto.Status == MembershipRequestStatus.Accepted)
                {
                    var user = request.User;
                    if (request.SelectedFromExisting)
                    {
                        var player = club.Players.SingleOrDefault(t => t.Nickname == request.Nickname);
                        if (player == null)
                            throw new ArgumentException("Requested member account not found");
                        player.User = user;
                        player.Nickname = request.Nickname;
                        player.LastChanged = DateTimeOffset.UtcNow;
                        user.PlayersAccounts.Add(player);
                    }
                    else
                    {
                        var newPlayer = new Player
                        {
                            ClubId = club.Id,
                            Guid = Guid.NewGuid(),
                            IsActive = true,
                            Registered = DateTimeOffset.UtcNow,
                            LastChanged = DateTimeOffset.UtcNow,
                            Nickname = request.Nickname,
                            UserId = user.Id
                        };
                        club.Players.Add(newPlayer);
                        user.PlayersAccounts.Add(newPlayer);
                    }
                }
                
                _unitOfWork.Save();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while fetching club membership requests. Please try a bit later");
            }
        }
    }
}
