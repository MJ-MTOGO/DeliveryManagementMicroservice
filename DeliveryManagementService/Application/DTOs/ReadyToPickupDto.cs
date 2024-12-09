namespace DeliveryManagementService.Application.DTOs
{
    public class ReadyToPickupDto
    {
        public Guid OrderId { get; set; }

        public ReadyToPickupDto(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
