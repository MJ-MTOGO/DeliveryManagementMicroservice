using DeliveryManagementService.Domain.ValueObjects;

namespace DeliveryManagementService.Domain.Entities
{
    public class OrderDelivering
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid RestaurantId { get; private set; }
        public Guid AgentId { get; private set; }
        public Adresse DeliveryAdresse { get; private set; }
        public Adresse PickupAdresse { get; private set; }
        public DateTime? DeliveringDatetime { get; private set; }

        public OrderDelivering(Guid orderId, Guid restaurantId, Guid agentId, Adresse deliveryAdresse, Adresse pickupAdresse)
        {
            Id = Guid.NewGuid();
            OrderId = orderId != Guid.Empty ? orderId : throw new ArgumentException("Order ID cannot be empty");
            RestaurantId = restaurantId != Guid.Empty ? restaurantId : throw new ArgumentException("Restaurant ID cannot be empty");
            AgentId = agentId != Guid.Empty ? agentId : throw new ArgumentException("Agent ID cannot be empty");
            DeliveryAdresse = deliveryAdresse ?? throw new ArgumentNullException(nameof(deliveryAdresse));
            PickupAdresse = pickupAdresse ?? throw new ArgumentNullException(nameof(pickupAdresse));
            DeliveringDatetime = null;
        }

        public void AssignAgent(Guid agentId)
        {
            if (agentId == Guid.Empty) throw new ArgumentException("Agent ID cannot be empty");
            AgentId = agentId;
        }

        public void UpdateDeliveringDatetime(DateTime deliveringDatetime)
        {
            if (deliveringDatetime == default)
                throw new ArgumentException("Delivering datetime cannot be empty or default");

            DeliveringDatetime = deliveringDatetime;
        }

        private OrderDelivering() { }
    }
}
