using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Helix;

namespace Hellbot.Service.Clients.Twitch
{
    public class TwitchClient
    {
        private readonly ILogger<TwitchClient> _logger;
        private readonly TwitchOptions _options;
        private readonly TwitchAPI _api = new();
        private readonly List<string> _scopes;
        public Helix API { get { return _api.Helix; } }
        public Auth Auth { get { return _api.Auth; } }

        public TwitchClient(ILogger<TwitchClient> logger, IOptions<TwitchOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            _api.Settings.ClientId = _options.API.ClientId;
            _scopes = new List<string>()
            {
                "user:read:chat",
                "user:write:chat",
                "moderator:read:chat_messages",
                "moderator:manage:banned_users",
                "moderator:read:chatters",
                "moderator:read:followers",
                "channel:manage:polls",
                "channel:manage:predictions",
                "channel:manage:redemptions",
                "channel:manage:broadcast",
                "channel:read:ads",
                "channel:read:subscriptions",
                "channel:edit:commercial",
                "clips:edit"
            };

            StartApi(_scopes).GetAwaiter().GetResult();
        }

        private async Task StartApi(List<string> scopes)
        {
            var server = new AuthServer(_options.API.RedirectUrl);
            var codeUrl = AuthServer.GetAuthorizationCodeUrl(_options.API.ClientId, _options.API.RedirectUrl, scopes);
            _logger.LogInformation($"Please authorize here:\n{codeUrl}");
            OpenBrowser(codeUrl);
            var auth = await server.Listen();
            var resp = await Auth.GetAccessTokenFromCodeAsync(auth?.Code, _options.API.ClientSecret, _options.API.RedirectUrl);
            _api.Settings.AccessToken = resp.AccessToken;
            var user = (await API.Users.GetUsersAsync()).Users[0];
            _logger.LogInformation($"Authorization success!\n\nUser: {user.DisplayName}({user.Id})\nScopes: {string.Join(", ", resp.Scopes)}");
        }

        private static void OpenBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
