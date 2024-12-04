namespace DeliveryManagementService.Application.DTOs
{
    public class OrderCreatedMessage
    {
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Address DeliveryAddress { get; set; }
    }
}
