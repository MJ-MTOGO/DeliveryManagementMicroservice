using DeliveryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DeliveryManagementService.Infrastructure
{
    public class DeliveringDbContext : DbContext
    {
        public DbSet<OrderDelivering> OrderDeliverings { get; set; }

        public DeliveringDbContext(DbContextOptions<DeliveringDbContext> options) : base(options) { }

        public static DbContextOptions<DeliveringDbContext> ConfigureDbContextOptions(string[] args = null)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DeliveringDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return optionsBuilder.Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDelivering>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.RestaurantId).IsRequired();
                entity.Property(e => e.AgentId).IsRequired();
                entity.Property(e => e.DeliveringDatetime).IsRequired(false);

                entity.OwnsOne(e => e.DeliveryAdresse, adresse =>
                {
                    adresse.Property(a => a.Street).HasColumnName("DeliveryStreet").IsRequired();
                    adresse.Property(a => a.PostalCode).HasColumnName("DeliveryPostalCode").IsRequired();
                    adresse.Property(a => a.City).HasColumnName("DeliveryCity").IsRequired();
                });

                entity.OwnsOne(e => e.PickupAdresse, adresse =>
                {
                    adresse.Property(a => a.Street).HasColumnName("PickupStreet").IsRequired();
                    adresse.Property(a => a.PostalCode).HasColumnName("PickupPostalCode").IsRequired();
                    adresse.Property(a => a.City).HasColumnName("PickupCity").IsRequired();
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
