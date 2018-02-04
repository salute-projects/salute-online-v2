using System;
using RawRabbit;
using SaluteOnline.API.Services.Interface;

namespace SaluteOnline.API.Services.Implementation
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
