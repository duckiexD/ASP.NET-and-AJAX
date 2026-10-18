using Microsoft.AspNetCore.SignalR;

namespace LiveMonitoringCenter;

public class MonitorHub : Hub
{
    private readonly ConnectionService _service;

    public MonitorHub(ConnectionService service)
    {
        _service = service;
    }

    // Задание 1, 3: при подключении — регистрация
    public override async Task OnConnectedAsync()
    {
        _service.AddConnection(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    // Задание 1, 3: при отключении — удаление
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _service.RemoveConnection(Context.ConnectionId);
        // НОВОЕ: уведомление с никнеймом (если он был установлен)
        var nickname = _service.GetNickname(Context.ConnectionId);
        await Clients.All.SendAsync("UserDisconnected", nickname);
        await base.OnDisconnectedAsync(exception);
    }

    // от себя: метод установки никнейма
    public async Task SetNickname(string nickname)
    {
        _service.SetNickname(Context.ConnectionId, nickname);
        // Уведомляем всех о новом пользователе с никнеймом
        await Clients.Others.SendAsync("UserConnected", nickname);
        // Подтверждаем клиенту, что никнейм принят
        await Clients.Caller.SendAsync("NicknameSet", nickname);
    }

    // Задание 1: широковещательная рассылка всем
    public async Task SendMessage(string text)
    {
        var nickname = _service.GetNickname(Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveMessage", nickname, text);
    }

    // Задание 2: вступление в комнату
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        _service.UpdateRoom(Context.ConnectionId, roomName);
        await Clients.Caller.SendAsync("RoomJoined", roomName);
    }

    // Задание 2: выход из комнаты
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        _service.UpdateRoom(Context.ConnectionId, null);
        await Clients.Caller.SendAsync("RoomLeft", roomName);
    }

    // Задание 2: отправка сообщения только в комнату
    public async Task SendMessageToRoom(string roomName, string text)
    {
        var nickname = _service.GetNickname(Context.ConnectionId);
        await Clients.Group(roomName).SendAsync("ReceiveRoomMessage", nickname, text);
    }

    // Задание 5: приватное сообщение конкретному клиенту
    public async Task SendPrivateMessage(string targetConnectionId, string text)
    {
        var nickname = _service.GetNickname(Context.ConnectionId);
        await Clients.Client(targetConnectionId).SendAsync("ReceivePrivateMessage", nickname, text);
        await Clients.Caller.SendAsync("ReceiveSystemMessage", $"Отправлено пользователю {targetConnectionId}");
    }
}