using DeliveryManagementService.Application.Ports;

namespace DeliveryManagementService.Infrastructure.Subscribers
{


    public class OrderCreatedSubscriptionHandler
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;

        public OrderCreatedSubscriptionHandler(IMessageBus messageBus, IServiceProvider serviceProvider)
        {
            _messageBus = messageBus;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()
        {
            await _messageBus.SubscribeAsync("order-created-sub", async message =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();

                                   
                    await orderProcessingService.ProcessOrderCreatedMessageAsync(message);
                }
            });
        }
    }


}
