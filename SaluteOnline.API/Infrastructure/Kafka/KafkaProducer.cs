using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.API.Infrastructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly Dictionary<string, object> _config;

        public KafkaProducer(IOptions<KafkaSettings> settings)
        {
            _config = new Dictionary<string, object>
            {
                { "bootstrap.servers", settings.Value.BootstrapServers },
                { "enable.auto.commit", false },
                { "message.send.max.retries", 3 },
                { "default.topic.config", new Dictionary<string, object>
                    {
                        { "message.timeout.ms", 3000 },
                        { "request.required.acks", "1" },
                    }
                }
            };
        }

        public async Task<bool> ProduceAsync(UserCreated message)
        {
            using (var producer = new Producer<Null, string>(_config, null, new StringSerializer(Encoding.UTF8)))
            {
                try
                {
                    var result = producer.ProduceAsync("my-topic", new Message<Null, string>
                    {
                        Value = JsonConvert.SerializeObject(message)
                    }).Result;
                    return !result.Error.HasError;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }
    }
}
