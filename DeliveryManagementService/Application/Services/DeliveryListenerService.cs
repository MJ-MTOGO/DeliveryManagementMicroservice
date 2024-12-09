using DeliveryManagementService.Infrastructure.WebSocketManagement;
using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;
using DeliveryManagementService.Application.DTOs;
using DeliveryManagementService.Application.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryManagementService.Application.Services
{
    public class DeliveryListenerService
    {
        private readonly DeliveringWebSocketManager _webSocketManager;
        private readonly HttpClient _httpClient;
        private readonly string _subscriptionId;
        private readonly string _projectId;
        private readonly IServiceProvider _serviceProvider;

        public DeliveryListenerService(
            DeliveringWebSocketManager webSocketManager,
            HttpClient httpClient,
            string projectId,
            string subscriptionId,
            IServiceProvider serviceProvider)
        {
            _webSocketManager = webSocketManager;
            _httpClient = httpClient;
            _projectId = projectId;
            _subscriptionId = subscriptionId;
            _serviceProvider = serviceProvider;
        }

        public async Task StartListeningAsync()
        {
            var subscriber = await SubscriberClient.CreateAsync(
                SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId));

            await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancellationToken) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();

                try
                {
                    var pubSubData = JsonConvert.DeserializeObject<OrderCreatedMessage>(message.Data.ToStringUtf8());
                    if (pubSubData?.OrderId == null)
                    {
                        Console.WriteLine("Invalid Pub/Sub message: OrderId is null.");
                        return SubscriberClient.Reply.Ack;
                    }

                    Console.WriteLine("Step 1. Order ID: " + pubSubData.OrderId);

                    await orderProcessingService.ProcessOrderCreatedMessageAsync(pubSubData);

                    Console.WriteLine("Step 2. Processed Order: " + pubSubData.OrderId);

                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    return SubscriberClient.Reply.Nack;
                }
            });
        }
    }
}

