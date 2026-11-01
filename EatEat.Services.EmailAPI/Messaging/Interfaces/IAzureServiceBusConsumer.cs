namespace EatEat.Services.EmailAPI.Messaging.Interfaces
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
