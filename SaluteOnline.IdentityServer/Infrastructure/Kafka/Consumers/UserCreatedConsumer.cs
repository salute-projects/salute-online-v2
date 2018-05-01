using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.IdentityServer.Infrastructure.Kafka.Consumers
{
    public class UserCreatedConsumer : IUserCreatedConsumer
    {
        private readonly Dictionary<string, object> _configuration;

        public UserCreatedConsumer(IOptions<KafkaSettings> settings)
        {
            var values = settings.Value;
            _configuration = new Dictionary<string, object>
            {
                { "group.id", values.GroupId },
                { "bootstrap.servers", values.BootstrapServers },
                { "enable.auto.commit", "false" },
                { "auto.commit.interval.ms", 5000 },
                { "auto.offset.reset", "earliest" }
            };
            Initialize();
        }

        public void Initialize()
        {
            using (var consumer = new Consumer<Null, string>(_configuration, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnMessage += (_, record) =>
                {
                    var x = record;
                    var tryObject = JsonConvert.DeserializeObject<UserCreated>(record.Value);
                };

                consumer.OnConsumeError += (sender, record) =>
                {

                };

                consumer.OnError += (sender, record) =>
                {

                };

                consumer.OnLog += (sender, record) =>
                {

                };

                consumer.OnOffsetsCommitted += (sender, record) =>
                {

                };

                consumer.OnPartitionEOF += (sender, record) =>
                {

                };

                //consumer.OnPartitionsAssigned += (sender, record) =>
                //{

                //};

                consumer.OnPartitionsRevoked += (sender, record) =>
                {

                };

                consumer.OnStatistics += (sender, record) =>
                {

                };

                consumer.Subscribe("my-topic");

                while (true)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}