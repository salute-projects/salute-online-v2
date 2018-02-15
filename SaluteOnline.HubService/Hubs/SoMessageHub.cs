using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.Exceptions;
using SaluteOnline.HubService.DAL;
using SaluteOnline.HubService.Domain;

namespace SaluteOnline.HubService.Hubs
{
    public class SoMessageHub : Hub
    {
        private readonly IGenericRepository<ConnectedUser> _usersRepository;

        public SoMessageHub(IGenericRepository<ConnectedUser> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.Claims.FirstOrDefault(t => t.Type == "subjectId")?.Value;
            if (!Guid.TryParse(userId, out var userGuid))
                throw new SoException("Malformed authorization token", HttpStatusCode.Unauthorized);
            var connectionId = Context.ConnectionId;
            var existingUser = _usersRepository.GetAsQueryable(t => t.Guid == userGuid)?.FirstOrDefault();
            if (existingUser == null)
            {
                _usersRepository.Insert(new ConnectedUser
                {
                    Connected = DateTimeOffset.UtcNow,
                    Guid = userGuid,
                    Connections = new List<SignalrConnection>
                    {
                        new SignalrConnection
                        {
                            Guid = Guid.NewGuid(),
                            Connected = true,
                            ConnectionId = connectionId
                        }
                    }
                });
            }
            else
            {
                if (existingUser.Connections.All(t => t.ConnectionId != connectionId))
                {
                    existingUser.Connections.Add(new SignalrConnection
                    {
                        Guid = Guid.NewGuid(),
                        ConnectionId = connectionId,
                        Connected = true
                    });
                }
            }

            return base.OnConnectedAsync();
        }

        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Claims.FirstOrDefault(t => t.Type == "subjectId")?.Value;
            if (!Guid.TryParse(userId, out var userGuid))
                throw new SoException("Malformed authorization token", HttpStatusCode.Unauthorized);
            var connectionId = Context.ConnectionId;
            var existingUser = _usersRepository.GetAsQueryable(t => t.Guid == userGuid)?.FirstOrDefault();
            if (existingUser == null)
                return base.OnDisconnectedAsync(exception);
            existingUser.Connections = existingUser.Connections.Where(t => t.ConnectionId != connectionId).ToList();
            if (existingUser.Connections.Any())
            {
                _usersRepository.Update(existingUser);
            }
            else
            {
                _usersRepository.Delete(existingUser);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
