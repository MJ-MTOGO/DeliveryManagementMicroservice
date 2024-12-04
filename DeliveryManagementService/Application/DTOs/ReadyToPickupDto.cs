namespace DeliveryManagementService.Application.DTOs
{
    public class ReadyToPickup
    {
        public Guid OrderId { get; set; }

        public ReadyToPickup(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
