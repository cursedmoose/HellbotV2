using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using PlayniteWebsocket.Client;
using System.Net.WebSockets;

namespace Hellbot.Service.Clients.Playnite
{
    public class PlayniteClient
    {
        private readonly PlayniteWebsocketClient _ws;
        private readonly ILogger<PlayniteClient> _logger;
        public PlayniteWebsocketClient Websocket { get => _ws; }
        private readonly PlayniteOptions _options;

        public PlayniteClient(IOptions<PlayniteOptions> options, ILogger<PlayniteClient> logger)
        {
            _options = options.Value;
            _ws = new(_options.WebsocketUrl);
            _logger = logger;

            _ws.Connected += () =>
            {
                _logger.LogInformation("Playnite Websocket Connected");
            };

            _ws.Disconnected += async () =>
            {
                _logger.LogInformation("Playnite Websocket disconnected.");
                while (_ws.Status != WebSocketState.Open)
                {
                    try
                    {
                        await _ws.Connect();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Playnite reconnect attempt failed.");
                    }

                    if (_ws.Status == WebSocketState.Open)
                        break;

                    await Task.Delay(3000);
                }
            };
        }

        public Task Connect() => _ws.Connect();
        public Task Disconnect() => _ws.Disconnect();

        public Task StartGame(Guid gameId)
        {
            return _ws.GetGameInfo(gameId);
        }

        public Task StopGame(Guid gameId)
        {
            return _ws.GetGameInfo(gameId);
        }
    }
}
