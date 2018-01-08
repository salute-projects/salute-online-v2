using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SaluteOnline.API.DAL;
using SaluteOnline.Domain.Domain.Mongo;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.API.Hub
{
    public class SoMessageHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public SoMessageHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public override Task OnConnectedAsync()
        {
            var email = Context.User.Claims.SingleOrDefault(t => t.Type == "email")?.Value;
            var connectionId = Context.ConnectionId;
            var existingUser = _unitOfWork.ConnectedUsers.Get(t => t.Email == email).FirstOrDefault();
            if (existingUser == null)
            {
                _unitOfWork.ConnectedUsers.Insert(new ConnectedUser
                {
                    Connected = DateTimeOffset.UtcNow,
                    Email = email,
                    Guid = Guid.NewGuid(),
                    Connections = new List<SignalrConnection>
                    {
                        new SignalrConnection
                        {
                            Connected = true,
                            Guid = Guid.NewGuid(),
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
                        Connected = true,
                        ConnectionId = connectionId,
                        Guid = Guid.NewGuid()
                    });
                }
            }
            lock (_unitOfWork)
            {
                _unitOfWork.Save();
            }
            return base.OnConnectedAsync();
        }

        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var email = Context.User.Claims.SingleOrDefault(t => t.Type == "email")?.Value;
            var connectionId = Context.ConnectionId;
            return Task.Delay(15000).ContinueWith(_ =>
            {
                var existingUser = _unitOfWork.ConnectedUsers.Get(t => t.Email == email).FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.Connections = existingUser.Connections.Where(t => t.ConnectionId != connectionId).ToList();
                    if (existingUser.Connections.Any())
                    {
                        _unitOfWork.ConnectedUsers.Update(existingUser);
                    }
                    else
                    {
                        _unitOfWork.ConnectedUsers.Delete(existingUser);
                    }
                }
                lock (_unitOfWork)
                {
                    _unitOfWork.Save();
                }
                return base.OnDisconnectedAsync(exception);
            });
        }
    }
}
