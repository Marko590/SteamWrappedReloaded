namespace SteamWrappedReloaded.Model
{
    public class GameModel
    {
        public string? Name { get; set; }
        public uint appId { get; set; }
        public TimeSpan playTime { get; set; }
        public TimeSpan? playTimeTwoWeeks { get; set; }
        public string? imgLogoUrl { get; set; }
    }
}
