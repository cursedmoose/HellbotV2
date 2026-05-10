using System.Diagnostics.CodeAnalysis;
using Hellbot.Core.Scenes;
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
        private readonly IOptionsMonitor<ObsOptions> _optionsMonitor;
        private int _reconnectOwned; // 1 while ReconnectLoopAsync owns reconnect; prevents stacked loops from Disconnected re-entry

        public ObsClient(OBSWebsocket obs, IOptionsMonitor<ObsOptions> optionsMonitor, ILogger<ObsClient> logger)
        {
            API = obs;
            _logger = logger;
            _optionsMonitor = optionsMonitor;

            obs.Connected += OnConnect;
            obs.Disconnected += OnDisconnect;
        }

        public void Start()
        {
            API.ConnectAsync(_optionsMonitor.CurrentValue.WebsocketUrl, "");
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
            if (Interlocked.CompareExchange(ref _reconnectOwned, 1, 0) != 0)
                return;

            _logger.LogInformation("OBS Websocket disconnected due to {Reason}.", e.DisconnectReason ?? "OBS is not running.");
            _ = ReconnectLoopAsync();
        }

        private async Task ReconnectLoopAsync()
        {
            try
            {
                while (!API.IsConnected)
                {
                    try
                    {
                        API.ConnectAsync(_optionsMonitor.CurrentValue.WebsocketUrl, "");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "OBS reconnect attempt failed.");
                    }

                    if (API.IsConnected)
                        break;

                    await Task.Delay(3000);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _reconnectOwned, 0);
            }
        }

        public void EnableScene(string? sceneId)
        {
            if (sceneId is null) return;

            var scenes = _optionsMonitor.CurrentValue.Scenes;
            if (TryGetSceneItem(scenes, sceneId, out var sceneItem))
            {
                API.SetSceneItemEnabled(sceneItem.Scene, sceneItem.ItemId, true);
            }
        }

        public void DisableScene(string? sceneId)
        {
            if (sceneId is null) return;

            var scenes = _optionsMonitor.CurrentValue.Scenes;
            if (TryGetSceneItem(scenes, sceneId, out var sceneItem))
            {
                API.SetSceneItemEnabled(sceneItem.Scene, sceneItem.ItemId, false);
            }
        }

        private static bool TryGetSceneItem(Dictionary<string, SceneItem> scenes, string sceneId, [NotNullWhen(true)] out SceneItem? sceneItem)
        {
            if (scenes.TryGetValue(sceneId, out sceneItem) && sceneItem is not null)
                return true;

            var withEquals = sceneId.Replace(':', '=');
            if (withEquals != sceneId && scenes.TryGetValue(withEquals, out sceneItem) && sceneItem is not null)
                return true;

            sceneItem = null;
            return false;
        }
    }
}
