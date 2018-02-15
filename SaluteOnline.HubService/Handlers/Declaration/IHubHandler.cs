using SaluteOnline.Domain.Events;

namespace SaluteOnline.HubService.Handlers.Declaration
{
    public interface IHubHandler
    {
        bool HandleEvent(HubEvent data);
    }
}