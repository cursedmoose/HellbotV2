using Hellbot.Service.Clients.Twitch;
using Hellbot.Service.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitchController(TwitchClient twitch, IOptions<TwitchOptions> twitchOptions) : ControllerBase
    {
        /// <summary>Comma-separated reward ids, or omit for all. Uses configured broadcaster.</summary>
        [HttpGet("channel-points/custom-rewards")]
        public async Task<IActionResult> GetCustomRewards(string? rewardIds, bool onlyManageable = false)
        {
            List<string>? ids = ToList(SplitCsv(rewardIds));
            return Ok(await twitch.API.ChannelPoints.GetCustomRewardAsync(twitchOptions.Value.BroadcasterId, ids, onlyManageable));
        }

        /// <summary>Omit broadcasterId for configured channel.</summary>
        [HttpGet("channel")]
        public async Task<IActionResult> GetChannel(string? broadcasterId)
        {
            var bid = string.IsNullOrWhiteSpace(broadcasterId)
                ? twitchOptions.Value.BroadcasterId
                : broadcasterId;
            return Ok(await twitch.API.Channels.GetChannelInformationAsync(broadcasterIds: [bid]));
        }

        /// <summary>Comma-separated Helix user ids and/or logins.</summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(string? ids, string? logins)
        {
            var idList = ToList(SplitCsv(ids));
            var loginList = ToList(SplitCsv(logins));
            return Ok(await twitch.API.Users.GetUsersAsync(idList, loginList));
        }

        private static List<string>? ToList(IReadOnlyList<string>? value)
        {
            if (value is null || value.Count == 0)
                return null;
            return [..value];
        }

        private static IReadOnlyList<string>? SplitCsv(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length == 0 ? null : parts;
        }
    }
}
