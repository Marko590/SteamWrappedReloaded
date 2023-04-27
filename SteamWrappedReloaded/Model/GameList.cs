namespace SteamWrappedReloaded.Model
{
    public class GameList
    {
        public IEnumerable<GameModel>? games { get; set; }
        public double percentOfUnplayed { get; set; }
    }
}
