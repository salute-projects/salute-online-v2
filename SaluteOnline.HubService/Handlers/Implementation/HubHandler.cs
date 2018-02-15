using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SaluteOnline.Domain.Events;
using SaluteOnline.HubService.Handlers.Declaration;
using SaluteOnline.HubService.Hubs;

namespace SaluteOnline.HubService.Handlers.Implementation
{
    public class HubHandler : IHubHandler
    {
        private readonly ILogger<HubHandler> _logger;
        private readonly IHubContext<SoMessageHub> _hub;

        public HubHandler(ILogger<HubHandler> logger, IHubContext<SoMessageHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public bool HandleEvent(HubEvent data)
        {
            try
            {
                if (data.ToAll)
                {
                    _hub.Clients.All.InvokeAsync(data.Method, data.Payload);
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}
