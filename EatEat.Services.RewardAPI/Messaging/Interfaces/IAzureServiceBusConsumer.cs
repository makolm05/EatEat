namespace EatEat.Services.RewardsAPI.Messaging.Interfaces
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
