namespace DeliveryManagementService.Application.Ports
{
    public interface IMessageBus
    {
        Task PublishAsync(string topic, object message);

        Task SubscribeAsync(string subscriptionName, Func<string, Task> messageProcessor);
    }

}
