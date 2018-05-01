using System.Threading.Tasks;

namespace SaluteOnline.IdentityServer.Infrastructure.Kafka
{
    public interface IKafkaProducer
    {
        Task<bool> ProduceAsync<TPayload>(string topic, TPayload message, string key = null);
    }
}