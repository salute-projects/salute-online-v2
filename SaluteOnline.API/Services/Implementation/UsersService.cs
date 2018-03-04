using System;
using System.Linq;
using System.Net;
using Mapster;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Domain;
using SaluteOnline.API.DTO.User;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Events;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Services.Implementation
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersService> _logger;
        private readonly IBusService _busService;

        public UsersService(IUnitOfWork unitOfWork, ILogger<UsersService> logger, IBusService busService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _busService = busService;
        }

        public Page<UserDto> GetUsers(UserFilter filter)
        {
            Func<User, bool> searchCriteria;
            try
            {
                searchCriteria = t => 
                    t.Role != Roles.SilentDon && t.Role != Roles.Guest
                    && (string.IsNullOrEmpty(filter.Name) ||  $"{t.FirstName} {t.LastName}".ToLower().Contains(filter.Name.ToLower())) 
                    && (string.IsNullOrEmpty(filter.Email) || t.Email.ToLower().Contains(filter.Email.ToLower()))
                    && (string.IsNullOrEmpty(filter.Nickname) || t.Nickname.ToLower().Contains(filter.Email.ToLower()))
                    && (string.IsNullOrEmpty(filter.City) || string.Equals(filter.City, t.City, StringComparison.CurrentCultureIgnoreCase))
                    && (string.IsNullOrEmpty(filter.Country) || string.Equals(filter.Country, t.Country, StringComparison.CurrentCultureIgnoreCase))
                    && (filter.Role == Roles.None || t.Role == filter.Role)
                    && (filter.Status == UserStatus.None || t.Status == filter.Status);
                var skip = filter.Page == 0 ? 0 : (filter.Page - 1) * (filter.PageSize ?? 25);
                var take = filter.PageSize ?? 25;
                var orderByField = string.IsNullOrEmpty(filter.OrderBy) ? nameof(User.Registered) : filter.OrderBy;
                var allUsers = _unitOfWork.Users.GetAsQueryable(t => searchCriteria(t));
                var slice = (filter.Asc ?
                    allUsers.OrderBy(t => typeof(User).GetProperty(orderByField).GetValue(t)) :
                    allUsers.OrderByDescending(t => typeof(User).GetProperty(orderByField).GetValue(t)))
                    .Skip(skip)
                    .Take(take);
                var allCount = allUsers.Select(t => t.Id).Count();
                var page = new Page<UserDto>(filter.Page, filter.PageSize ?? 25, allCount, slice.Select(t => t.Adapt<UserDto>()));
                return page;
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while getting users list. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void SetUserRole(SetRoleRequest request)
        {
            try
            {
                if (request.Role == Roles.SilentDon || request.Role == Roles.Guest || request.Role == Roles.None)
                    throw new SoException("Invalid request", HttpStatusCode.BadRequest);

                var existing = _unitOfWork.Users.GetById(id: request.UserId);
                if (existing == null)
                    throw new SoException("User not found", HttpStatusCode.BadRequest);

                existing.Role = request.Role;
                existing.LastActivity = DateTimeOffset.UtcNow;
                _unitOfWork.Save();
                _busService.Publish(new UserRoleChangeEvent
                {
                    SubjectId = existing.Guid.ToString(),
                    Role = request.Role
                });
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while getting users list. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }
    }
}
