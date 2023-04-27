namespace SteamWrappedReloaded.Model
{
    public class Friend
    {
        public string? userName { get; set; }
        public string? avatarUrl { get; set; }
        public ulong steamId { get; set; }
        public string? relationship { get; set; }
        public DateTime friendsSince { get; set; }

        public string? currentGameName { get; set; }

        public string? currentGameId { get; set; }
    }
}
