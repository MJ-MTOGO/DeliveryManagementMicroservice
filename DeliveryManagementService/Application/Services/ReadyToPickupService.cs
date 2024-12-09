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
            var readyToPickup = jsonObject["readyToPickup"]?.ToObject<ReadyToPickupDto>();

            if (readyToPickup == null)
            {
                throw new Exception("Failed to deserialize message");
            }

          
             var deliveringOrder = await _repository.FindOrderDeliveringByOrderIdAsync(readyToPickup.OrderId);

            // Create DeliveringInfoDto
            var deliveringInfoDto = new DeliveringInfoDto
            {
                OrderStatus = "ReadyToPickup",
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
