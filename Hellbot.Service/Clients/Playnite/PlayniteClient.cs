using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using PlayniteWebsocket.Client;

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

            _ws.Disconnected += () =>
            {
                _logger.LogInformation("Playnite Websocket Disconnected");
            };
        }

        public Task Connect() => _ws.Connect();
        public Task Disconnect() => _ws.Disconnect();

        public Task StartGame(Guid gameId)
        {
            return _ws.SendCommand("StartGame", new { gameId });
        }

        public Task StopGame(Guid gameId)
        {
            return _ws.SendCommand("StopGame", new { gameId });
        }
    }
}
