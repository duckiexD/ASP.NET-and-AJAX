using LiveMonitoringCenter;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Подготовка: CORS для SignalR
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

// Подготовка: регистрация SignalR
builder.Services.AddSignalR();

// Задание 3: регистрация сервиса подключений (singleton)
builder.Services.AddSingleton<ConnectionService>();

var app = builder.Build();

// Подготовка: middleware CORS
app.UseCors();

// Подготовка: статические файлы из wwwroot
app.UseStaticFiles();

// Подготовка: маппинг хаба на /monitorhub
app.MapHub<MonitorHub>("/monitorhub");

// Задание 3: HTTP POST для системного сообщения всем
app.MapPost("/api/system/message", async (ConnectionService service) =>
{
    await service.SendSystemMessageAsync("Системное сообщение через HTTP POST");
    return Results.Ok();
});

// Задание 5: HTTP POST для уведомления в комнату
app.MapPost("/api/room/{roomName}/notify", async (string roomName, string text, ConnectionService service) =>
{
    await service.SendMessageToRoomAsync(roomName, "Server", $"[ВНЕШНЕЕ] {text}");
    return Results.Ok();
});

// Задание 5: HTTP POST для приватного сообщения клиенту
app.MapPost("/api/user/{connectionId}/notify", async (string connectionId, string text, ConnectionService service) =>
{
    await service.SendMessageToUserAsync(connectionId, "Server", $"[ВНЕШНЕЕ ПРИВАТНОЕ] {text}");
    return Results.Ok();
});

app.Run();