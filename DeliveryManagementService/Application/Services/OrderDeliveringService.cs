using DeliveryManagementService.Application.DTOs;
using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Domain.Entities;

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


        public async Task UpdateDeliveringTimeAsync(Guid id)
        {
            // Get the current time
            DateTime deliveringDatetime = DateTime.UtcNow;

            // Update delivery time
            await _repository.UpdateDeliveringDatetimeAsync(id, deliveringDatetime);
            var orderDelivering = await _repository.FindByOrderIdAsync(id);
            // You can include the message publishing logic here if needed
            //  Publish the "order-delivered" message
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
