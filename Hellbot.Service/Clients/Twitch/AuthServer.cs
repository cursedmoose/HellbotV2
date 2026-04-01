using System.Net;

namespace Hellbot.Service.Clients.Twitch
{
    internal class Authorization(string code)
    {
        public string Code { get; } = code;
    }
    internal class AuthServer
    {
        private readonly HttpListener listener;

        public AuthServer(string uri)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
        }

        public async Task<Authorization?> Listen()
        {
            listener.Start();
            return await OnRequest();
        }

        private async Task<Authorization?> OnRequest()
        {
            while (listener.IsListening)
            {
                var ctx = await listener.GetContextAsync();
                var req = ctx.Request;
                var resp = ctx.Response;

                using var writer = new StreamWriter(resp.OutputStream);
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable CS8604 // Possible null reference argument.
                if (req.QueryString.AllKeys.Any("code".Contains))
                {
                    writer.WriteLine("Authorization started! Check your application!");
                    writer.Flush();
                    return new Authorization(req.QueryString["code"]);
                }
                else
                {
                    writer.WriteLine("No code found in query string!");
                    writer.Flush();
                }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            }
            return null;
        }

        public static string GetAuthorizationCodeUrl(string clientId, string redirectUri, List<string> scopes)
        {
            var scopesStr = String.Join('+', scopes);

            return "https://id.twitch.tv/oauth2/authorize?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={System.Web.HttpUtility.UrlEncode(redirectUri)}&" +
                   "response_type=code&" +
                   $"scope={scopesStr}";
        }
    }
}
