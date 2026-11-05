using System.Text;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using EatEat.Services.RewardAPI.Messages;
using EatEat.Services.RewardsAPI.Services;
using EatEat.Services.RewardsAPI.Messaging.Interfaces;

namespace EatEat.Services.RewardsAPI.Messaging
{
    // better if realise as IHostedService
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedRewardsSubscription;
        private readonly string _serviceBusConnectionString;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _rewardProcessor;
        private readonly RewardsService _rewardsService;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardsService rewardsService)
        {
            _configuration = configuration;

            _serviceBusConnectionString = _configuration.GetConnectionString("AzureServiceBusConnectionString");
            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueues:OrderCreatedTopic");
            _orderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueues:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedRewardsSubscription);

            _rewardsService = rewardsService;
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRequestReceivedAsync;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;

            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        private async Task OnNewOrderRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            var text = Encoding.UTF8.GetString(args.Message.Body);
            var email = JsonConvert.DeserializeObject<RewardsMessage>(text);

            try
            {
                //TODO - try log email
                await _rewardsService.UpdateRewards(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}