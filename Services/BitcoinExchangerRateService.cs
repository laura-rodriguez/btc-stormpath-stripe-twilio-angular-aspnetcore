using Newtonsoft.Json;
using stormpath_angularjs_dotnet_stripe_twilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace stormpath_angularjs_dotnet_stripe_twilio.Services
{
    public class BitcoinExchangeRateService
    {
        public async Task<decimal> GetBitcoinExchangeRate()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("http://api.bitcoincharts.com/v1/weighted_prices.json");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Failed to retrieve BTC exchange rates");
            }

            var responseBody = JsonConvert.DeserializeObject<BTCExchangeRateResponse>(await response.Content.ReadAsStringAsync());

            if (responseBody.USD == null || responseBody.USD.Last24H == null)
            {
                throw new Exception("Failed to retrieve BTC exchange rates");
            }

            return responseBody.USD.Last24H.Value;
        }
    }
}
