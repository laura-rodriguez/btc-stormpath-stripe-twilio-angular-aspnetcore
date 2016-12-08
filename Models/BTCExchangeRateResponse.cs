using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stormpath_angularjs_dotnet_stripe_twilio.Models
{
    public class BTCExchangeRateResponse
    {
        public BTCExchangeRateCurrency USD { get; set; }
    }

    public class BTCExchangeRateCurrency
    {
        [JsonProperty("24h")]
        public decimal? Last24H { get; set; }
    }
}
