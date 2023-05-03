using Microsoft.AspNetCore.Mvc;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWrappedReloaded.Model;

namespace SteamWrappedReloaded
{

    public class SteamService : BaseService
    {
        GameService gameService = new GameService();
        public ActionResult<UserModel> GetUser(ulong steamId)
        {
            var playerSummaryResponse = steamInterface.GetPlayerSummaryAsync(steamId).Result.Data;

            var retVal= new UserModel
            {
                status = playerSummaryResponse.UserStatus,
                UserName = playerSummaryResponse.Nickname,
                avatarUrl = playerSummaryResponse.AvatarFullUrl,
                realName = playerSummaryResponse.RealName,
            };
            return new OkObjectResult(retVal);
        }

        // Change to steamInteface.GetPlayerSummariesAsync()

        public ActionResult<IEnumerable<Friend>> GetFriends(ulong steamId, int? numberOfFriends)
        {
            var friendListResponse = steamInterface.GetFriendsListAsync(steamId);
            IReadOnlyCollection<FriendModel> friendListData;
            try
            {
                friendListData = friendListResponse.Result.Data;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult("");
            }

            int friendFetchLimit;
            if (!numberOfFriends.HasValue)
            {
                friendFetchLimit = friendListData.Count;
            }
            else
            {
                friendFetchLimit = numberOfFriends.Value;
            }

            int fetchCounter = 0;

            var returnValue = new List<Friend>();
            foreach (FriendModel friend in friendListData)
            {
                if (fetchCounter == friendFetchLimit)
                    break;

                fetchCounter++;

                var playerSummaryResponse = steamInterface.GetPlayerSummaryAsync(friend.SteamId).Result.Data;
                returnValue.Add(new Friend
                {
                    status = playerSummaryResponse.UserStatus,
                    currentGameId = playerSummaryResponse.PlayingGameId,
                    currentGameName = playerSummaryResponse.PlayingGameName,
                    userName = playerSummaryResponse.Nickname,
                    avatarUrl = playerSummaryResponse.AvatarFullUrl,
                    friendsSince = friend.FriendSince,
                    steamId = friend.SteamId,
                    relationship = friend.Relationship
                });
            }
            return new OkObjectResult(returnValue);
        }

        public ActionResult<GameList> GetGames(ulong steamId, int? numberOfGames)
        {
            var steamPlayerInterface = webInterfaceFactory.CreateSteamWebInterface<PlayerService>(client);

            var ownedGamesResponse = steamPlayerInterface.GetOwnedGamesAsync(steamId, includeAppInfo: true, includeFreeGames: true);
            OwnedGamesResultModel? ownedGames;

            try
            {
                ownedGames = ownedGamesResponse.Result.Data;
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult("");
            }

            if (!numberOfGames.HasValue)
            {
                numberOfGames = (int)ownedGames.GameCount;
            }
            

            int fetchCounter = 0;
            var gamesList = new List<GameModel>();
            foreach (OwnedGameModel game in ownedGames.OwnedGames)
            {
                if (fetchCounter == numberOfGames.Value)
                    break;
                var appInfo = gameService.GetAppInfo(game.AppId);

                fetchCounter++;
                gamesList.Add(new GameModel
                {
                    appId = game.AppId,
                    imgLogoUrl = getImageForGame(game.AppId, game.ImgIconUrl),
                    Name = game.Name,
                    playTime = game.PlaytimeForever,
                    playTimeTwoWeeks = game.PlaytimeLastTwoWeeks,
                    price = appInfo.price,
                    coverUrl = appInfo.coverUrl
                });
            }

            var returnValue = new GameList
            {
                games = gamesList.OrderByDescending(o => o.playTime),
                percentOfUnplayed = GetPercentOfUnplayed(ownedGames)
            };
            return new OkObjectResult(returnValue);
        }



        public ActionResult<IEnumerable<RecentGameModel>> GetRecentGames(ulong steamId)
        {
            var steamPlayerInterface = webInterfaceFactory.CreateSteamWebInterface<PlayerService>(client);

            var recentGamesResponse = steamPlayerInterface.GetRecentlyPlayedGamesAsync(steamId);

            var recentGames = recentGamesResponse.Result.Data;

            List<RecentGameModel> gamesList = new List<RecentGameModel>();
            foreach (RecentlyPlayedGameModel recentGame in recentGames.RecentlyPlayedGames)
            {

                gamesList.Add(new RecentGameModel
                {
                    allTime = recentGame.PlaytimeForever,
                    appId = recentGame.AppId,
                    headerImage = gameService.getHeaderForGame(recentGame.AppId),
                    twoWeeks = recentGame.Playtime2Weeks,
                    Name = recentGame.Name
                }
                 );
            }
            return gamesList;

        }

        private string getImageForGame(uint appId, string logoUrl)
        {
            return String.Format("https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/{0}/{1}.jpg", appId.ToString(), logoUrl);
        }



        private double GetPercentOfUnplayed(OwnedGamesResultModel? ownedGames)
        {
            int numberOfUnplayed = 0;
            foreach (OwnedGameModel game in ownedGames.OwnedGames)
            {
                if (game.PlaytimeForever.TotalSeconds == 0)
                {
                    numberOfUnplayed++;
                }
            }
            return ((double)numberOfUnplayed / (double)ownedGames.GameCount) * 100;

        }
    }
}
