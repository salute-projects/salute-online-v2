using RawRabbit;
using SaluteOnline.IdentityServer.Service.Declaration;

namespace SaluteOnline.IdentityServer.Service.Implementation
{
    public class BusService : IBusService
    {
        private readonly IBusClient _busClient;

        public BusService(IBusClient busClient)
        {
            _busClient = busClient;
        }

        public void Publish<TEntity>(TEntity message)
        {
            if (message == null)
                return;
            _busClient.PublishAsync(message);
        }
    }
}
