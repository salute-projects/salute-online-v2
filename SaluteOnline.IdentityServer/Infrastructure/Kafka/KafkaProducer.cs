using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Options;

namespace SaluteOnline.IdentityServer.Infrastructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly Dictionary<string, object> _config;

        public KafkaProducer(IOptions<KafkaSettings> settings)
        {
            _config = new Dictionary<string, object>
            {
                { "bootstrap.servers", settings.Value.BootstrapServers }
            };
        }

        public async Task<bool> ProduceAsync<TPayload>(string topic, TPayload message, string key = null)
        {
            var payloadSerializer = new AvroSerializer<TPayload>();
            var keySerializer = new AvroSerializer<string>();

            using (var producer = new Producer<string, TPayload>(_config, keySerializer, payloadSerializer))
            {
                var result = await producer.ProduceAsync("my-topic", null, message);
                return !result.Error.HasError;
            }
        }
    }
}
