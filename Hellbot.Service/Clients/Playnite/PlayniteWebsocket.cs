using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Hellbot.Service.Clients.Playnite
{
    public class PlayniteGameStartedEvent
    {
        public string Game { get; set; }
    }
    public class PlayniteWebsocket(string url)
    {
        private readonly Uri _uri = new Uri(url);
        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;

        public event Action<PlayniteGameStartedEvent> GameStarted;
        public event Action<string> RawMessage;
        public event Action Connected;
        public event Action Disconnected;

        public async Task Connect()
        {
            _cts = new CancellationTokenSource();
            _ws = new ClientWebSocket();

            await _ws.ConnectAsync(_uri, _cts.Token);
            Connected?.Invoke();

            _ = Task.Run(ReceiveLoop);
        }

        public async Task SendCommand(string name, string game = null)
        {
            var payload = new
            {
                type = "command",
                name,
                game
            };

            var json = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, _cts.Token);
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[8192];

            try
            {
                while (_ws.State == WebSocketState.Open)
                {
                    var result = await _ws.ReceiveAsync(buffer, _cts.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    RawMessage?.Invoke(msg);

                    HandleMessage(msg);
                }
            }
            catch
            {
                // keep silent for v0
            }

            Disconnected?.Invoke();
        }

        private void HandleMessage(string msg)
        {
            var json = JsonDocument.Parse(msg);
            var root = json.RootElement;

            if (!root.TryGetProperty("type", out var typeProp))
                return;

            var type = typeProp.GetString();

            if (type == "gameStarted")
            {
                var game = root.GetProperty("game").GetString();

                GameStarted?.Invoke(new PlayniteGameStartedEvent
                {
                    Game = game
                });
            }
        }

        public async Task Disconnect()
        {
            try
            {
                _cts?.Cancel();

                if (_ws != null && _ws.State == WebSocketState.Open)
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
            }
            catch { }

            Disconnected?.Invoke();
        }
    }
}
