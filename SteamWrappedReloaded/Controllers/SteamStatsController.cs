using Microsoft.AspNetCore.Mvc;
using SteamWebAPI2.Interfaces;
using SteamWrappedReloaded.Model;

namespace SteamWrappedReloaded.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SteamStatsController : ControllerBase
    {

        private SteamService steamService = new SteamService();
        private GameService gameService = new GameService();
        private readonly ILogger<SteamStatsController> _logger;

        public SteamStatsController(ILogger<SteamStatsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUser")]
        public ActionResult<UserModel> GetUsername(ulong steamId)
        {
            return steamService.GetUser(steamId);
        }

        [HttpGet]
        [Route("friends")]
        public ActionResult<IEnumerable<Friend>> GetFriends(ulong steamId,int? numberOfFriends)
        {
            return steamService.GetFriends(steamId, numberOfFriends);
        }

        [HttpGet]
        [Route("games")]
        public ActionResult<GameList> GetGames(ulong steamId, int? numberOfGames)
        {
            return steamService.GetGames(steamId, numberOfGames);
        }

        [HttpGet]
        [Route("recent")]
        public ActionResult<IEnumerable<RecentGameModel>> GetRecentGames(ulong steamId)
        {
            return steamService.GetRecentGames(steamId);
        }
    }
}