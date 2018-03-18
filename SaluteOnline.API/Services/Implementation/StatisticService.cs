using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.DTO.Statistic;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Services.Implementation
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StatisticService> _logger;

        public StatisticService(IUnitOfWork unitOfWork, ILogger<StatisticService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IEnumerable<ClubDashboardStatisticDto> GetMyClubsStatistics(string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                var currentUser = _unitOfWork.Users.GetById(userGuid);
                if (currentUser == null)
                    throw new SoException("User not found", HttpStatusCode.BadRequest);

                var clubs =
                    _unitOfWork.Clubs.GetAsQueryable(t => t.Administrators.Any(x => x.UserId == currentUser.Id))
                        .Include(t => t.Administrators)
                        .ThenInclude(t => t.User)
                        .Include(t => t.MembershipRequests)
                        .Include(t => t.Players)
                        .ThenInclude(t => t.User);

                return clubs.Select(t => new ClubDashboardStatisticDto
                {
                    Players = t.Players.Count,
                    ClubId = t.Id,
                    ClubTitle = t.Title,
                    MembershipRequests = t.MembershipRequests.Count
                });
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while loading club statistic. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }
    }
}
