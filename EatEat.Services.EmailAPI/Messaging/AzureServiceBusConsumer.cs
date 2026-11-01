using Azure.Messaging.ServiceBus;
using EatEat.Services.EmailAPI.Messaging.Interfaces;
using EatEat.Services.EmailAPI.Models.Dto;
using EatEat.Services.EmailAPI.Services;
using EatEat.Services.EmailAPI.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace EatEat.Services.EmailAPI.Messaging
{
    // better if realise as IHostedService
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _emailCartQueue;
        private readonly string _emailRegisterQueue;
        private readonly string _serviceBusConnectionString;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emailCartProcessor;
        private readonly ServiceBusProcessor _emailRegisterProcessor;
        private readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;

            _serviceBusConnectionString = _configuration.GetConnectionString("AzureServiceBusConnectionString");
            _emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            _emailRegisterQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(_emailCartQueue);
            _emailRegisterProcessor = client.CreateProcessor(_emailRegisterQueue);

            _emailService = emailService;
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceivedAsync;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailCartProcessor.StartProcessingAsync();

            _emailRegisterProcessor.ProcessMessageAsync += OnRegisterRequestReceivedAsync;
            _emailRegisterProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailRegisterProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _emailRegisterProcessor.StopProcessingAsync();
            await _emailRegisterProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            var text = Encoding.UTF8.GetString(args.Message.Body);
            var email = JsonConvert.DeserializeObject<CartDto>(text);

            try
            {
                //TODO - try log email
                await _emailService.EmailCart(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task OnRegisterRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            var text = Encoding.UTF8.GetString(args.Message.Body);
            var cartDto = JsonConvert.DeserializeObject<string>(text);

            try
            {
                //TODO - try log email
                await _emailService.RegisterUserEmail(cartDto);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
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