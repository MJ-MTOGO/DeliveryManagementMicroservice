namespace DeliveryManagementService.Application.Ports
{
    public interface IOrderProcessingService
    {
        Task ProcessOrderCreatedMessageAsync(string messageData);
    }
}
