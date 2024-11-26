using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using DeliveryManagementService.Application.Ports;

namespace DeliveryManagementService.Infrastructure.Publishers
{
    public class GooglePubSubMessageBus : IMessageBus
    {
        private readonly PublisherServiceApiClient _publisher;
        private readonly SubscriberServiceApiClient _subscriber;
        private readonly string _projectId;

        public GooglePubSubMessageBus(string projectId)
        {
            _projectId = projectId;

            // Initialize the Google Pub/Sub publisher and subscriber clients
            _publisher = PublisherServiceApiClient.Create();
            _subscriber = SubscriberServiceApiClient.Create(); // Ensure the subscriber is created
        }

        public async Task PublishAsync(string topicName, object message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var topic = TopicName.FromProjectTopic(_projectId, topicName);

            var pubsubMessage = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(jsonMessage)
            };

            await _publisher.PublishAsync(topic, new[] { pubsubMessage });
        }

        public async Task SubscribeAsync(string subscriptionName, Func<string, Task> messageProcessor)
        {
            var subscription = SubscriptionName.FromProjectSubscription(_projectId, subscriptionName);

            while (true)
            {
                try
                {
                    var response = await _subscriber.PullAsync(subscription, maxMessages: 10);
                    foreach (var message in response.ReceivedMessages)
                    {
                        try
                        {
                            var messageData = message.Message.Data.ToStringUtf8();

                            // Process the message using the provided callback
                            await messageProcessor(messageData);

                            // Acknowledge the message
                            await _subscriber.AcknowledgeAsync(subscription, new[] { message.AckId });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscribing to messages: {ex.Message}");
                }
            }
        }
    }
}
