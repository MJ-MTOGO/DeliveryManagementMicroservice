namespace DeliveryManagementService.Application.Ports
{
    public interface IReadyToPickupService
    {
        Task ProcessReadyToPickupMessageAsync(string messageData);
    }
}
