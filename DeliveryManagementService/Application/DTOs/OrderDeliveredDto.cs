namespace DeliveryManagementService.Application.DTOs
{
    public class OrderDeliveredDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid AgentId { get; set; }
        public DateTime DeliveringDatetime { get; set; }
        public string DeliveringAddress { get; set; }
    }
}
