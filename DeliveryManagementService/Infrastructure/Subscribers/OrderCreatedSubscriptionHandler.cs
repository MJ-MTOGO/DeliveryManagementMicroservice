using DeliveryManagementService.Application.DTOs;
using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;

public class OrderCreatedSubscriptionHandler
{
    private readonly string _subscriptionId;
    private readonly string _projectId;

    public OrderCreatedSubscriptionHandler(string projectId, string subscriptionId)
    {
        _projectId = projectId;
        _subscriptionId = subscriptionId;
    }

    public async Task StartListeningAsync()
    {
        var subscriber = await SubscriberClient.CreateAsync(
            SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId));

        Console.WriteLine("************************************************* StartListeningAsync");

        await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancellationToken) =>
        {
            try
            {
                var pubSubData = JsonConvert.DeserializeObject<OrderCreatedMessage>(message.Data.ToStringUtf8());
                if (pubSubData?.OrderId == null)
                {
                    Console.WriteLine("Invalid Pub/Sub message: OrderId is null.");
                    return SubscriberClient.Reply.Ack;
                }
                Console.WriteLine("Step 1. Order ID: " + pubSubData.OrderId);

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



//var orderProcessingService = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();
//Console.WriteLine($"Processing message: {message}");
//await orderProcessingService.ProcessOrderCreatedMessageAsync(message);
