using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AsynchronousPrograming.POCOs;

namespace AsynchronousPrograming
{
    /// <summary>
    /// Vasim ukolem bude ziskat predpoved pocasi ze 3 ruznych API.
    /// Pro usnadneni prace mate jiz predchystane ukazkove URL requesty na
    /// dilci sluzby. K dispozici mate take dalsi pomocne metody.
    /// 
    /// I.   Upravte metodu PerformRequest tak, aby pozadavek zasilala asynchrone
    /// 
    /// II.  Dokoncete metodu GetWeatherAsync dle dokumentace
    /// 
    /// III. Implementujte metodu TestAsync, tato metoda vytvori 3 asynchroni 
    ///      requesty, ktere se hned po dokonceni asynchroni operace zpracuji
    ///      (pouzijte metodu ContinueWith(...), kterou zavolate na prislusnem 
    ///      Tasku) a vysledek se vypise na konzoli (viz predchystane metody v
    ///      regionu JsonProcessing)
    /// 
    /// </summary>
    public static class Solution
    {
        #region wheatherApiUrls
        /// <summary>
        /// Poskytovatel OpenWeather: max 100 pozadavku za minutu / url - v pripade prekroceni limitu vyuzijte OpenWeatherApi02
        /// </summary>
        private const string OpenWeatherApi01 = "http://api.openweathermap.org/data/2.5/weather?q=Brno&units=metric&APPID=d9fee349c9ee04bb29dea054bbf09555";
        private const string OpenWeatherApi02 = "http://api.openweathermap.org/data/2.5/weather?q=Brno&units=metric&APPID=6924b1d7b6bafd9214b4e6e8e5740969";

        /// <summary>
        /// Poskytovatel WeatherUnlocked: max 60 pozadavku za minutu / url - v pripade prekroceni limitu vyuzijte WeatherUnlockedApi02
        /// </summary>
        private const string WeatherUnlockedApi01 = "http://api.weatherunlocked.com/api/current/49.19,16.60?app_id=bcfb6773&app_key=222a3c7ed30850d0e86c708f01b6948a";
        private const string WeatherUnlockedApi02 = "http://api.weatherunlocked.com/api/current/49.19,16.60?app_id=3bfd31af&app_key=3f4a4eea942a3436a6f1b134e8244bff";

        /// <summary>
        /// Poskytovatel YahooWeatherApi: neomezeno
        /// </summary>
        private const string YahooWeatherApi = "https://query.yahooapis.com/v1/public/yql?q=select%20item.condition.text%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22brno%2C%20cz%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys"; 
        #endregion

        /// <summary>
        /// Shows current wheather based on OpenWeather, WeatherUnlocked and YahooWeather Api
        /// </summary>
        public static async Task TestAsync()
        {
            await Task.WhenAll(
                GetWeatherAsync(WeatherUnlockedApi01).ContinueWith(apiCallTask =>
                {
                    if (apiCallTask.IsFaulted)
                    {
                        Console.WriteLine($"Failed to obtain Brno forecast from WheatherUnlocked.{Environment.NewLine}{apiCallTask.Exception?.Message}");
                        return;
                    }
                    var wheatherUnlockedJson = apiCallTask.Result;
                    ProcessWheatherUnlockedJson(wheatherUnlockedJson);
                }), 
                GetWeatherAsync(OpenWeatherApi01).ContinueWith(apiCallTask =>
                {
                    if (apiCallTask.IsFaulted)
                    {
                        Console.WriteLine($"Failed to obtain Brno forecast from OpenWeather.{Environment.NewLine}{apiCallTask.Exception?.Message}");
                        return;
                    }
                    var openWheatherJson = apiCallTask.Result;
                    ProcessOpenWheatherJson(openWheatherJson);
                }), 
                GetWeatherAsync(YahooWeatherApi).ContinueWith(apiCallTask =>
                {
                    if (apiCallTask.IsFaulted)
                    {
                        Console.WriteLine($"Failed to obtain Brno forecast from Yahoo.{Environment.NewLine}{apiCallTask.Exception?.Message}");
                        return;
                    }
                    var yahooWheatherJson = apiCallTask.Result;
                    ProcessYahooJson(yahooWheatherJson);
                }));
        }

        /// <summary>
        /// Gets wheather info according to given parameter
        /// </summary>
        /// <param name="apiUrlString">Url string to make request to</param>
        /// <returns>Specific json with wheather info</returns>
        private static async Task<string> GetWeatherAsync(string apiUrlString)
        {
            var apiUrl = ParseUrl(apiUrlString);
            var apiName = apiUrlString.Split('.')[1];
            var weatherJson = string.Empty;
            try
            {
                weatherJson = await PerformRequest(apiName, apiUrl);
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Download failed due to {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception has been thrown due to {ex.Message}");
            }
            return weatherJson;
        }

        /// <summary>
        /// Performs request to API according to given apiUrl
        /// </summary>
        /// <param name="apiName">Short name of the api (used for logging)</param>
        /// <param name="apiUrl">Url to make request to</param>
        /// <returns>Specific json with wheather info</returns>
        private static async Task<string> PerformRequest(string apiName, Uri apiUrl)
        {
            using (var webClient = new WebClient())
            {
                Console.WriteLine($"Performed API call: {apiName}{Environment.NewLine}");
                webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                webClient.Headers["Accept"] = "application/json";
                var weatherJson = await webClient.DownloadStringTaskAsync(apiUrl);
                Console.WriteLine($"Received response from call: {apiName}{Environment.NewLine}");
                return weatherJson;
            }
        }

        /// <summary>
        /// Parses given url into Uri
        /// </summary>
        /// <param name="apiUrlString">Url string to parse</param>
        /// <returns>Parsed url</returns>
        private static Uri ParseUrl(string apiUrlString)
        {
            Uri apiUrl;
            var isUrlValid = Uri.TryCreate(apiUrlString, UriKind.Absolute, out apiUrl)
                             && (apiUrl.Scheme == Uri.UriSchemeHttp || apiUrl.Scheme == Uri.UriSchemeHttps);
            if (!isUrlValid)
            {
                throw new UriFormatException("Given string is not valid url.");
            }
            return apiUrl;
        }

        #region JsonProcessing
        /// <summary>
        /// Prints wheather report (from WheatherUnlockedApi) from given json
        /// </summary>
        /// <param name="wheatherUnlockedJson">Json from WheatherUnlockedApi containing the wheather report</param>
        private static void ProcessWheatherUnlockedJson(string wheatherUnlockedJson)
        {
            var wheatherUnlockedInfo =
                Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessWheatherUnlockedResponse>(wheatherUnlockedJson);
            Console.WriteLine(
                $"Brno forecast from WheatherUnlocked:{Environment.NewLine}{wheatherUnlockedInfo?.wx_desc}{Environment.NewLine}temp: {wheatherUnlockedInfo?.temp_c}°C{Environment.NewLine}humidity: {wheatherUnlockedInfo?.humid_pct}%{Environment.NewLine}");
        }

        /// <summary>
        /// Prints wheather report (from OpenWheatherApi) from given json
        /// </summary>
        /// <param name="openWheatherJson">Json from OpenWheatherApi containing the wheather report</param>
        private static void ProcessOpenWheatherJson(string openWheatherJson)
        {
            var openWheatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessOpenWheatherResponse2>(openWheatherJson);
            Console.WriteLine(
                $"Brno forecast from OpenWheather:{Environment.NewLine}{openWheatherInfo?.weather.FirstOrDefault()?.description}{Environment.NewLine}temp: {openWheatherInfo?.main.temp}°C{Environment.NewLine}humidity: {openWheatherInfo?.main.humidity}%{Environment.NewLine}");
        }

        /// <summary>
        /// Prints wheather report (from YahooWheatherApi) from given json
        /// </summary>
        /// <param name="yahooWheatherJson">Json from YahooWheatherApi containing the wheather report</param>
        private static void ProcessYahooJson(string yahooWheatherJson)
        {
            var yahooWheatherInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<YahooWeatherPoco>(yahooWheatherJson);
            Console.WriteLine(
                $"Brno forecast from Yahoo:{Environment.NewLine}{yahooWheatherInfo?.query?.results?.channel?.item?.condition.text}{Environment.NewLine}");
            if (yahooWheatherInfo?.query?.results == null)
            {
                Console.WriteLine("<No data available>");
            }
        } 
        #endregion
    }
}
