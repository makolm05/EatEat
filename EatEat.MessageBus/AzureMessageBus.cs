using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace EatEat.MessageBus
{
    public class AzureMessageBus : IMessageBus
    {
        private readonly string _connectionString;

        public AzureMessageBus(IOptions<AzureServiceBusSettings> opt)
        {
            _connectionString = opt.Value.AzureServiceBusConnectionString;
        }


        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(_connectionString);
            var sender = client.CreateSender(topic_queue_name);

            var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);
        }
    }
}