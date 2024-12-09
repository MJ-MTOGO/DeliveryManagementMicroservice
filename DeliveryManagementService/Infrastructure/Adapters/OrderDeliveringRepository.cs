using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManagementService.Infrastructure.Adapters
{
    public class OrderDeliveringRepository : IOrderDeliveringRepository
    {
        private readonly DeliveringDbContext _dbContext;

        public OrderDeliveringRepository(DeliveringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveOrderDeliveringAsync(OrderDelivering orderDelivering)
        {
            _dbContext.OrderDeliverings.Add(orderDelivering);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderDelivering> FindByIdAsync(Guid id)
        {
            return await _dbContext.OrderDeliverings.FirstOrDefaultAsync(d => d.Id == id)
                   ?? throw new InvalidOperationException($"Order with ID {id} not found");
        }

          public async Task UpdateDeliveringDatetimeAsync(Guid id, DateTime deliveringDatetime)
        {
            var orderDelivering = await FindByOrderIdAsync(id);
            orderDelivering.UpdateDeliveringDatetime(deliveringDatetime);
            _dbContext.OrderDeliverings.Update(orderDelivering);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<OrderDelivering> FindOrderDeliveringByOrderIdAsync(Guid id)
        {
            return await _dbContext.OrderDeliverings.FirstOrDefaultAsync(d => d.OrderId == id)
                   ?? throw new InvalidOperationException($"Order with ID {id} not found");
        }

        public async Task UpdateOrderDeliveringAsync(OrderDelivering orderDelivering)
        {
            _dbContext.OrderDeliverings.Update(orderDelivering);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OrderDelivering> FindByOrderIdAsync(Guid id)
        {
            return await _dbContext.OrderDeliverings.FirstOrDefaultAsync(d => d.OrderId == id)
                 ?? throw new InvalidOperationException($"Order with ID {id} not found");
        }
    }
}
