using DeliveryManagementService.Application.DTOs;
using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Infrastructure.WebSocketManagement;
using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeliveryManagementService.Infrastructure.Subscribers
{
    public class ReadyToPickupSubscriptionHandler
    {
        private readonly DeliveringWebSocketManager _webSocketManager;
        private readonly HttpClient _httpClient;
        private readonly string _subscriptionId;
        private readonly string _projectId;
        private readonly IServiceProvider _serviceProvider;

        public ReadyToPickupSubscriptionHandler(
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
            try
            {
                Console.WriteLine($"Initializing subscriber for {_subscriptionId}...");
                var subscriber = await SubscriberClient.CreateAsync(
                    SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId));

                await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancellationToken) =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var orderProcessingService = scope.ServiceProvider.GetService<IOrderProcessingService>();

                    if (orderProcessingService == null)
                    {
                        Console.WriteLine("IOrderProcessingService is not registered.");
                        return SubscriberClient.Reply.Nack;
                    }

                    try
                    {
                        //var pubSubData = JsonConvert.DeserializeObject<OrderCreatedMessage>(message.Data.ToStringUtf8());
                        var jsonObject = JObject.Parse(message.Data.ToStringUtf8());
                        var pubSubData = jsonObject["readyToPickup"]?.ToObject<ReadyToPickupDto>();
                        if (pubSubData?.OrderId == null)
                        {
                            Console.WriteLine("Invalid Pub/Sub message: OrderId is null.");
                            return SubscriberClient.Reply.Ack;
                        }

                        Console.WriteLine($"Processing OrderId: {pubSubData.OrderId}");

                        await orderProcessingService.ProcessOrderUpdatedMessageAsync(pubSubData);

                        Console.WriteLine($"Successfully processed OrderId: {pubSubData.OrderId}");
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        return SubscriberClient.Reply.Nack;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize subscriber: {ex.Message}");
            }
        }
    }
}
