using DeliveryManagementService.Domain.Entities;
using DeliveryManagementService.Domain.ValueObjects;

namespace DeliveryManagementService.Domain;

public class Class
{
    private readonly List<OrderDelivering> _deliveries = new();

    public IReadOnlyCollection<OrderDelivering> Deliveries => _deliveries.AsReadOnly();

    public OrderDelivering CreateOrderDelivering(
        Guid orderId,
        Guid restaurantId,
        Guid agentId,
        Adresse deliveryAdresse,
        Adresse pickupAdresse)
    {
        var orderDelivering = new OrderDelivering(orderId, restaurantId, agentId, deliveryAdresse, pickupAdresse);
        _deliveries.Add(orderDelivering);
        return orderDelivering;
    }

    public OrderDelivering FindDeliveryById(Guid id)
    {
        return _deliveries.FirstOrDefault(d => d.Id == id)
               ?? throw new InvalidOperationException($"Delivery with ID {id} not found");
    }
}
