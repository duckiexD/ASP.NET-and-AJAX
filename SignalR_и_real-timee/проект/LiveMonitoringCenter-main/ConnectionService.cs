using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace LiveMonitoringCenter;

public class ConnectionService
{
    private readonly IHubContext<MonitorHub> _hubContext;
    // Задание 3: словарь подключений (ConnectionId → комната)
    private static readonly ConcurrentDictionary<string, string?> _connections = new();
    
    // от себя: словарь никнеймов (ConnectionId → никнейм)
    private static readonly ConcurrentDictionary<string, string> _nicknames = new();

    public ConnectionService(IHubContext<MonitorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Задание 3: регистрация нового подключения
    public void AddConnection(string connectionId)
    {
        _connections.TryAdd(connectionId, null);
        BroadcastOnlineCount();
    }

    // Задание 3: удаление подключения
    public void RemoveConnection(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
        _nicknames.TryRemove(connectionId, out _); // НОВОЕ: удаляем никнейм
        BroadcastOnlineCount();
    }

    // Задание 3: обновление комнаты для подключения
    public void UpdateRoom(string connectionId, string? roomName)
    {
        if (_connections.ContainsKey(connectionId))
            _connections[connectionId] = roomName;
    }

    // от себя: установка никнейма
    public void SetNickname(string connectionId, string nickname)
    {
        _nicknames[connectionId] = nickname;
    }

    // от себя: получение никнейма (если нет — возвращает "Аноним")
    public string GetNickname(string connectionId)
    {
        return _nicknames.TryGetValue(connectionId, out var nick) ? nick : "Аноним";
    }

    public int GetOnlineCount() => _connections.Count;

    // Задание 3: рассылка счётчика онлайн всем
    private void BroadcastOnlineCount()
    {
        _hubContext.Clients.All.SendAsync("UpdateOnlineCount", GetOnlineCount());
    }

    // от себя: рассылка всем с никнеймом отправителя
    public async Task BroadcastWithNicknameAsync(string senderId, string eventName, params object[] args)
    {
        var nickname = GetNickname(senderId);
        await _hubContext.Clients.All.SendAsync(eventName, nickname, args);
    }

    // Задание 3: системное сообщение всем через контекст хаба
    public async Task SendSystemMessageAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveSystemMessage", message);
    }

    // Задание 2, 5: отправка сообщения в комнату
    public async Task SendMessageToRoomAsync(string roomName, string senderId, string text)
    {
        var nickname = GetNickname(senderId);
        await _hubContext.Clients.Group(roomName).SendAsync("ReceiveRoomMessage", nickname, text);
    }

    // Задание 5: приватное сообщение конкретному клиенту
    public async Task SendMessageToUserAsync(string connectionId, string senderId, string text)
    {
        var nickname = GetNickname(senderId);
        await _hubContext.Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", nickname, text);
    }
}