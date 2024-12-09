using Microsoft.EntityFrameworkCore;
using DeliveryManagementService.Application.Ports;
using DeliveryManagementService.Application.Services;
using DeliveryManagementService.Infrastructure.Publishers;
using DeliveryManagementService.Infrastructure.WebSocketManagement;
using DeliveryManagementService.Infrastructure.Adapters;
using DeliveryManagementService.Infrastructure;
using DeliveryManagementService.Infrastructure.Subscribers;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Database Context with connection string
builder.Services.AddDbContext<DeliveringDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// Register IMessageBus with GooglePubSubMessageBus
builder.Services.AddSingleton<IMessageBus, GooglePubSubMessageBus>(sp =>
    new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

// Register IMessagePublisher with MessagePublisher
builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

// Register WebSocketManager
builder.Services.AddSingleton<DeliveringWebSocketManager>();

// Register HttpClient for Subscription Handlers
builder.Services.AddHttpClient();

// Register Repositories and Services
builder.Services.AddScoped<IOrderDeliveringRepository, OrderDeliveringRepository>();
builder.Services.AddScoped<OrderDeliveringService>();
builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();

// Register DeliveryListenerService with all required dependencies
builder.Services.AddSingleton<DeliveryListenerService>(sp =>
{
    Console.WriteLine("Initializing DeliveryListenerService...");
    var webSocketManager = sp.GetRequiredService<DeliveringWebSocketManager>();
    var httpClient = sp.GetRequiredService<HttpClient>();
    var projectId = builder.Configuration["GoogleCloud:ProjectId"];
    var subscriptionId = builder.Configuration["GoogleCloud:SubscriptionId"];
    var serviceProvider = sp; // Pass IServiceProvider for scoped service resolution

    return new DeliveryListenerService(
        webSocketManager,
        httpClient,
        projectId,
        subscriptionId,
        serviceProvider
    );
});

// Register ReadyToPickupSubscriptionHandler with all required dependencies
builder.Services.AddSingleton<ReadyToPickupSubscriptionHandler>(sp =>
{
    Console.WriteLine("Initializing ReadyToPickupSubscriptionHandler...");
    var webSocketManager = sp.GetRequiredService<DeliveringWebSocketManager>();
    var httpClient = sp.GetRequiredService<HttpClient>();
    var projectId = builder.Configuration["GoogleCloud:ProjectId"];
    var subscriptionId = builder.Configuration["GoogleCloud:SubscriptionIdReady"];
    var serviceProvider = sp; // Pass IServiceProvider for scoped service resolution

    return new ReadyToPickupSubscriptionHandler(
        webSocketManager,
        httpClient,
        projectId,
        subscriptionId,
        serviceProvider
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS (optional)
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

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

app.MapControllers();

// Start DeliveryListenerService
Console.WriteLine("Starting DeliveryListenerService...");
var deliveryListenerService = app.Services.GetRequiredService<DeliveryListenerService>();
_ = deliveryListenerService.StartListeningAsync(); // Fire-and-forget task

// Start ReadyToPickupSubscriptionHandler
Console.WriteLine("Starting ReadyToPickupSubscriptionHandler...");
var readyToPickupHandler = app.Services.GetRequiredService<ReadyToPickupSubscriptionHandler>();
_ = readyToPickupHandler.StartListeningAsync(); // Fire-and-forget task

app.Run();
