using DeliveryManagementService.Application.DTOs;

namespace DeliveryManagementService.Application.Ports
{
    public interface IOrderProcessingService
    {
        Task ProcessOrderCreatedMessageAsync(OrderCreatedMessage messageData);
        Task ProcessOrderUpdatedMessageAsync(ReadyToPickupDto messageData);
    }
}
