using DeliveryManagementService.Application.DTOs;
using DeliveryManagementService.Application.Ports;

namespace DeliveryManagementService.Application.Services
{
    public class OrderDeliveringService
    {
        private readonly IOrderDeliveringRepository _repository;
        private readonly IMessageBus _messageBus;

        public OrderDeliveringService(IOrderDeliveringRepository repository, IMessageBus messageBus)
        {
            _repository = repository;
            _messageBus = messageBus;
        }

        public async Task UpdateDeliveringTimeAsync(Guid id, DateTime deliveringDatetime)
        {
            // Update delivery time
            var orderDelivering = await _repository.FindByIdAsync(id);
            orderDelivering.UpdateDeliveringDatetime(deliveringDatetime);
            await _repository.UpdateOrderDeliveringAsync(orderDelivering);

            // Publish the "order-delivered" message
            var orderDeliveredDto = new OrderDeliveredDto
            {
                Id = orderDelivering.Id,
                OrderId = orderDelivering.OrderId,
                AgentId = orderDelivering.AgentId,
                DeliveringDatetime = orderDelivering.DeliveringDatetime.Value,
                DeliveringAddress = $"{orderDelivering.DeliveryAdresse.Street}, {orderDelivering.DeliveryAdresse.City}, {orderDelivering.DeliveryAdresse.PostalCode}"
            };

            await _messageBus.PublishAsync("order-delivered", orderDeliveredDto);
        }
    }
}
