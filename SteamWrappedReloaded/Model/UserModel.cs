using Steam.Models.SteamCommunity;

namespace SteamWrappedReloaded.Model
{
    public class UserModel
    {
        public string? UserName { get; set; }

        public string? realName { get; set; }

        public DateTime? lastLoggedOf { get; set; }

        public string? avatarUrl { get; set; }

    }
}
