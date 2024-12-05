using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Application.Services;
using DeliveryManagementService.Infrastructure;
using DeliveryManagementService.Infrastructure.Adapters;
using DeliveryManagementService.Infrastructure.Publishers;
using DeliveryManagementService.Infrastructure.Subscribers;
using DeliveryManagementService.Infrastructure.WebSocketManagement;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManagementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Services
            ConfigureServices(builder);

            var app = builder.Build();

            // Configure Middleware
            ConfigureMiddleware(app);

            // Start Subscription Services
            StartSubscriptions(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Configure DbContext
            builder.Services.AddDbContext<DeliveringDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register IMessageBus with GooglePubSubMessageBus
            builder.Services.AddSingleton<IMessageBus, GooglePubSubMessageBus>(sp =>
                new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

            // Register IMessagePublisher with MessagePublisher
            builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

            // Register WebSocketManager
            builder.Services.AddSingleton<DeliveringWebSocketManager>();

            // Add Repositories and Services
            builder.Services.AddScoped<IOrderDeliveringRepository, OrderDeliveringRepository>();
            builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();

            // Add Subscription Handlers as Singletons, resolving scoped dependencies dynamically
            builder.Services.AddSingleton<OrderCreatedSubscriptionHandler>();

            // Register Controllers
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register HttpClient
            builder.Services.AddHttpClient();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            // Enable Swagger in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Add WebSocket Middleware
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var webSocketManager = context.RequestServices.GetRequiredService<DeliveringWebSocketManager>();
                        await webSocketManager.HandleWebSocketConnectionAsync(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400; // Bad Request
                    }
                }
                else
                {
                    await next();
                }
            });

            // Map Controllers
            app.MapControllers();
        }

        private static void StartSubscriptions(WebApplication app)
        {
            // Start listening to the "order-created-sub" subscription
            var orderCreatedHandler = app.Services.GetRequiredService<OrderCreatedSubscriptionHandler>();
            _ = orderCreatedHandler.StartAsync(); // Fire-and-forget task
        }
    }
}
