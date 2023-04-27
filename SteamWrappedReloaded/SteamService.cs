using Microsoft.AspNetCore.Mvc;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWrappedReloaded.Model;

namespace SteamWrappedReloaded
{

    public class SteamService : BaseService
    { 
        public UserModel GetUser(ulong steamId)
        {
            var playerSummaryResponse = steamInterface.GetPlayerSummaryAsync(steamId).Result.Data;

            return new UserModel
            {
                UserName = playerSummaryResponse.Nickname,
                avatarUrl = playerSummaryResponse.AvatarFullUrl,
                realName = playerSummaryResponse.RealName,
            };
        }

        // Change to steamInteface.GetPlayerSummariesAsync()

        public ActionResult<IEnumerable<Friend>> GetFriends(ulong steamId, int numberOfFriends)
        {
            var friendListResponse = steamInterface.GetFriendsListAsync(steamId).Result.Data;
            
            var friends = new List<Friend>();
            var friendFetchLimit = numberOfFriends;
            int fetchCounter = 0;

            foreach (FriendModel friend in friendListResponse)
            {
                if (fetchCounter == friendFetchLimit)
                    break;

                fetchCounter++;
                
                var playerSummaryResponse = steamInterface.GetPlayerSummaryAsync(friend.SteamId).Result.Data;

                friends.Add(new Friend
                {
                    currentGameId = playerSummaryResponse.PlayingGameId,
                    currentGameName = playerSummaryResponse.PlayingGameName,
                    userName = playerSummaryResponse.Nickname,
                    avatarUrl = playerSummaryResponse.AvatarFullUrl,
                    friendsSince = friend.FriendSince,
                    steamId = friend.SteamId,
                    relationship = friend.Relationship
                });
            }
            return new ActionResult<IEnumerable<Friend>>(friends);
        }

        public GameList GetGames(ulong steamId, int numberOfGames) {
            var steamPlayerInterface = webInterfaceFactory.CreateSteamWebInterface<PlayerService>(client);
            
            var ownedGames = steamPlayerInterface.GetOwnedGamesAsync(steamId, includeAppInfo: true).Result.Data;

            var gamesList = new List<GameModel>();
            int fetchCounter = 0;
            foreach (OwnedGameModel game in ownedGames.OwnedGames)
            {
                if (fetchCounter == numberOfGames)
                    break;

                fetchCounter++;
                gamesList.Add(new GameModel
                {
                    appId = game.AppId,
                    imgLogoUrl = getImageForGame(game.AppId, game.ImgIconUrl),
                    Name = game.Name,
                    playTime = game.PlaytimeForever,
                    playTimeTwoWeeks = game.PlaytimeLastTwoWeeks

                });
            }

            return new GameList
            {
                games = gamesList,
                percentOfUnplayed = GetPercentOfUnplayed(ownedGames)
            };
        }


        private string getImageForGame(uint appId,string logoUrl)
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
            return ((double)numberOfUnplayed/(double)ownedGames.GameCount)*100;
            
        }
    }
}
