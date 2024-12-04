using DeliveryManagementService.Domain.ValueObjects;

namespace DeliveryManagementService.Application.DTOs
{
    public class DeliveringInfoDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid AgentId { get; set; }
        public string OrderStatus { get; set; }
        public Adresse DeliveryAdresse { get;  set; }
        public Adresse PickupAdresse { get;  set; }
    }
}
