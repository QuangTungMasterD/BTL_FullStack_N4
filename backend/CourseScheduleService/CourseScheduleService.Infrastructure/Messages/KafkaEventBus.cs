using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CourseScheduleService.Application.Interfaces.EventBus;
using Microsoft.Extensions.Configuration;

namespace CourseScheduleService.Infrastructure.Messages
{
    public class KafkaEventBus : IEventBus
    {
        private readonly IProducer<string, string> _producer;

        public KafkaEventBus(IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task PublishAsync<T>(string topic, T @event) where T : class
        {
            var json = JsonSerializer.Serialize(@event);
            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json,
                Headers = new Headers
                {
                    { "Type", System.Text.Encoding.UTF8.GetBytes(typeof(T).Name) }
                }
            };
            await _producer.ProduceAsync(topic, message);
        }
    }
}