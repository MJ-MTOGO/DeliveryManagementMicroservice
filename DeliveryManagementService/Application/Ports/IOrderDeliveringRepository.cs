using DeliveryManagementService.Domain.Entities;

namespace DeliveryManagementService.Application.Ports
{
    public interface IOrderDeliveringRepository
    {
        Task SaveOrderDeliveringAsync(OrderDelivering orderDelivering);
        Task<OrderDelivering> FindByIdAsync(Guid id); // For retrieving the entity
        Task<OrderDelivering> FindByOrderIdAsync(Guid id); // For retrieving the entity
        Task<OrderDelivering> FindOrderDeliveringByOrderIdAsync(Guid id); // For retrieving the entity
        Task UpdateDeliveringDatetimeAsync(Guid id, DateTime deliveringDatetime); // New method
        Task UpdateOrderDeliveringAsync(OrderDelivering orderDelivering); // New method
    }
}
