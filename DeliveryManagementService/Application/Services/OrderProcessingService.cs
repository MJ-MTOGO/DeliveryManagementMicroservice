using DeliveryManagementService.Application.DTOs;
using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Domain.Entities;
using DeliveryManagementService.Domain.ValueObjects;
using DeliveryManagementService.Infrastructure.WebSocketManagement;
using Newtonsoft.Json;

namespace DeliveryManagementService.Application.Services
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly HttpClient _httpClient;
        private readonly IOrderDeliveringRepository _repository;
        private readonly DeliveringWebSocketManager _webSocketManager;

        public OrderProcessingService(HttpClient httpClient, IOrderDeliveringRepository repository, DeliveringWebSocketManager webSocketManager)
        {
            _httpClient = httpClient;
            _repository = repository;
            _webSocketManager = webSocketManager;
        }

        public async Task ProcessOrderCreatedMessageAsync(OrderCreatedMessage messageData)
        {
            // Deserialize the message
          
            Console.WriteLine("******************************************************************************************************************");
            Console.WriteLine(messageData);
            Console.WriteLine("******************************************************************************************************************");
            if (messageData == null)
            {
                throw new Exception("Failed to deserialize message");
            }

            // Make an API request to get restaurant information
            var restaurantResponse = await _httpClient.GetAsync($"http://localhost:5139/api/Restaurant/{messageData.RestaurantId}");
            restaurantResponse.EnsureSuccessStatusCode();
      
            Console.WriteLine(messageData.RestaurantId);
            var restaurantData = await restaurantResponse.Content.ReadAsStringAsync();
            var restaurantDto = JsonConvert.DeserializeObject<RestaurantDto>(restaurantData);

            if (restaurantDto == null || restaurantDto.Address == null)
            {
                throw new Exception("Failed to deserialize restaurant data");
            }

            // Map the DTO to Value Object
            var pickupAddress = new Adresse(
                restaurantDto.Address.Street,
                restaurantDto.Address.PostalCode,
                restaurantDto.Address.City
            );

            // Create OrderDelivering aggregate
            var deliveryAddress = new Adresse(
                messageData.DeliveryAddress.Street,
                messageData.DeliveryAddress.PostalCode,
                messageData.DeliveryAddress.City
            );
            var agentId = Guid.Parse("1AA2AA99-A5A3-4043-80C3-1CE7A64C3132");
            var orderDelivering = new OrderDelivering(
                messageData.OrderId,
                messageData.RestaurantId,
                agentId,
                deliveryAddress,
                pickupAddress
            );

            // Create DeliveringInfoDto
            var deliveringInfoDto = new DeliveringInfoDto
            {
                OrderStatus = "Pending",
                AgentId = orderDelivering.AgentId,
                OrderId = orderDelivering.OrderId,
                DeliveryAdresse = orderDelivering.DeliveryAdresse,
                Id = orderDelivering.Id,
                PickupAdresse = orderDelivering.PickupAdresse,
                RestaurantId = orderDelivering.RestaurantId
            };

            // Send the DTO to the agent via WebSocket
            var messageJson = JsonConvert.SerializeObject(deliveringInfoDto);
            await _webSocketManager.BroadcastMessageAsync(messageJson);

            // Save to the database
            await _repository.SaveOrderDeliveringAsync(orderDelivering);
        }

     
         public async Task ProcessOrderUpdatedMessageAsync(ReadyToPickupDto messageData)
        {
            // Deserialize the message

            Console.WriteLine("******************************************************************************************************************");
            Console.WriteLine(messageData);
            Console.WriteLine("******************************************************************************************************************");
            if (messageData == null)
            {
                throw new Exception("Failed to deserialize message");
            }
        

            // get orderdelivering from database and then send it again with readytopickup
            var orderDelivering = await _repository.FindByOrderIdAsync(messageData.OrderId);

            // Create DeliveringInfoDto
            var deliveringInfoDto = new DeliveringInfoDto
            {
                OrderStatus = "ReadyToPickup",
                AgentId = orderDelivering.AgentId,
                OrderId = orderDelivering.OrderId,
                DeliveryAdresse = orderDelivering.DeliveryAdresse,
                Id = orderDelivering.Id,
                PickupAdresse = orderDelivering.PickupAdresse,
                RestaurantId = orderDelivering.RestaurantId
            };

            // Send the DTO to the agent via WebSocket
            var messageJson = JsonConvert.SerializeObject(deliveringInfoDto);
            await _webSocketManager.BroadcastMessageAsync(messageJson);

        }
    }
}
