using Microsoft.AspNetCore.Mvc;
using SteamWebAPI2.Interfaces;
using SteamWrappedReloaded.Model;

namespace SteamWrappedReloaded
{
    public class GameService : BaseService
    {
        SteamStore? _store;
        public GameService()
        {
            _store = webInterfaceFactory.CreateSteamStoreInterface(client);
        }


        public string getHeaderForGame(uint appId)
        {
            var appInfoResponse = _store.GetStoreAppDetailsAsync(appId);
            try
            {
                var appInfo = appInfoResponse.Result;
                return appInfo.HeaderImage;
            }

            catch (Exception e)
            {

                Console.WriteLine(e);
                return "";
            }
        }

        public AdditionalGameInfo GetAppInfo(uint appId)
        {

            var appInfoResponse = _store.GetStoreAppDetailsAsync(appId);
            try
            {
                var appInfo = appInfoResponse.Result;
                float price;
                if (appInfo.IsFree)
                {
                    price = 0;
                }
                else
                {
                    if (appInfo.PriceOverview != null)
                    {
                        price = (float)appInfo.PriceOverview.Initial / 100;

                    }
                    else
                    {
                        price = 0;
                    }
                }
                return new AdditionalGameInfo { price = price, coverUrl = appInfo.HeaderImage };

            }

            catch (Exception e)
            {

                Console.WriteLine(e);
                return new AdditionalGameInfo { coverUrl = "", price = 0 };
            }
        }
    }
}
