using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;

namespace Hellbot.Service.Clients.OBS
{
    public class ObsClient
    {
        public readonly OBSWebsocket API;
        private readonly ILogger<ObsClient> _logger;
        private readonly ObsOptions _options;

        public ObsClient(OBSWebsocket obs, IOptions<ObsOptions> options, ILogger<ObsClient> logger)
        {
            API = obs;
            _logger = logger;
            _options = options.Value;

            obs.Connected += OnConnect;
            obs.Disconnected += OnDisconnect;
        }

        public void Start()
        {
            API.ConnectAsync(_options.WebsocketUrl, "");
        }

        public void Stop()
        {
            API.Disconnect();
        }

        private void OnConnect(object? sender, EventArgs e)
        {
            _logger.LogInformation("OBS Websocket connected.");
        }

        private void OnDisconnect(object? sender, ObsDisconnectionInfo e)
        {
            _logger.LogInformation("OBS Websocket disconnected due to {Reason}.", e.DisconnectReason ?? "OBS is not running.");
        }
    }
}
