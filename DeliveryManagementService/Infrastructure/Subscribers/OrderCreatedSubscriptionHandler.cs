﻿using DeliveryManagementService.Application.Ports;

namespace DeliveryManagementService.Infrastructure.Subscribers
{
    public class OrderCreatedSubscriptionHandler
    {
        private readonly IMessageBus _messageBus;
        private readonly IOrderProcessingService _orderProcessingService;

        public OrderCreatedSubscriptionHandler(IMessageBus messageBus, IOrderProcessingService orderProcessingService)
        {
            _messageBus = messageBus;
            _orderProcessingService = orderProcessingService;
        }

        public async Task StartAsync()
        {
            // Use the IMessageBus to subscribe to the "order-created-sub"
            await _messageBus.SubscribeAsync("order-created-sub", async messageData =>
            {
                try
                {
                    // Delegate the message processing to the application service
                    await _orderProcessingService.ProcessOrderCreatedMessageAsync(messageData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            });



        }
    }
}