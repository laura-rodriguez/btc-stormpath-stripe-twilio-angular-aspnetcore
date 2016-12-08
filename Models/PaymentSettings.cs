using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stormpath_angularjs_dotnet_stripe_twilio.Models
{
    public class PaymentSettings
    {
        public string StripePublicKey { get; set; }
        public string StripePrivateKey { get; set; }
    }
}
