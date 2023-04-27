using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace SteamWrappedReloaded
{
    public abstract class BaseService
    {
        protected static SteamWebInterfaceFactory webInterfaceFactory = new SteamWebInterfaceFactory(System.Environment.GetEnvironmentVariable("STEAM_DEV_TOKEN"));
        protected HttpClient client = new HttpClient();
        protected SteamUser? steamInterface;
        public BaseService()
        {
            steamInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>(client);
        }
    }
}
