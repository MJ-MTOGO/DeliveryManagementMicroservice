using DeliveryManagementService.Application.DTOs;
using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Infrastructure.WebSocketManagement;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DeliveryManagementService.Application.Services
{
    public class ReadyToPickupService : IReadyToPickupService
    {
        private readonly HttpClient _httpClient;
        private readonly IOrderDeliveringRepository _repository;
        private readonly DeliveringWebSocketManager _webSocketManager;

        public ReadyToPickupService(HttpClient httpClient, IOrderDeliveringRepository repository, DeliveringWebSocketManager webSocketManager)
        {
            _httpClient = httpClient;
            _repository = repository;
            _webSocketManager = webSocketManager;
        }

        public async Task ProcessReadyToPickupMessageAsync(string messageData)
        {
            // Deserialize the message
            var jsonObject = JObject.Parse(messageData);
            var readyToPickup = jsonObject["readyToPickup"]?.ToObject<ReadyToPickup>();

            if (readyToPickup == null)
            {
                throw new Exception("Failed to deserialize message");
            }

            // Fetch order details
            var orderResponse = await _httpClient.GetAsync($"http://localhost:5194/api/orders/{readyToPickup.OrderId}");
            orderResponse.EnsureSuccessStatusCode();

            var orderData = await orderResponse.Content.ReadAsStringAsync();
            var orderDto = JsonConvert.DeserializeObject<OrderDto>(orderData);

            var deliveringOrder = await _repository.FindOrderDeliveringByOrderIdAsync(orderDto.OrderId);

            // Create DeliveringInfoDto
            var deliveringInfoDto = new DeliveringInfoDto
            {
                OrderStatus = orderDto.OrderStatus,
                AgentId = deliveringOrder.AgentId,
                OrderId = deliveringOrder.OrderId,
                DeliveryAdresse = deliveringOrder.DeliveryAdresse,
                Id = deliveringOrder.Id,
                PickupAdresse = deliveringOrder.PickupAdresse,
                RestaurantId = deliveringOrder.RestaurantId
            };

            // Send the DTO to the agent via WebSocket
            var messageJson = JsonConvert.SerializeObject(deliveringInfoDto);
            await _webSocketManager.BroadcastMessageAsync(messageJson);
        }
    }
}
