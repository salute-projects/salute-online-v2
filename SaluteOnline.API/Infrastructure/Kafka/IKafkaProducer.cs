using System.Threading.Tasks;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.API.Infrastructure.Kafka
{
    public interface IKafkaProducer
    {
        Task<bool> ProduceAsync(UserCreated message);
    }
}