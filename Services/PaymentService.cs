using Microsoft.Extensions.Options;
using stormpath_angularjs_dotnet_stripe_twilio.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stormpath_angularjs_dotnet_stripe_twilio.Services
{
    public class PaymentService
    {
        private readonly PaymentSettings paymentSettings;
        private readonly StripeChargeService stripeChargeService;
        public static readonly int FIXED_AMOUNT = 2000;
        public static readonly int FIXED_COST_PER_QUERY = 2;

        public PaymentService(IOptions<PaymentSettings> paymentSettings, StripeChargeService stripeChargeService)
        {
            this.paymentSettings = paymentSettings.Value;
            this.stripeChargeService = stripeChargeService;
        }
        

        public bool ProcessPayment(string token, int amount)
        {
            var myCharge = new StripeChargeCreateOptions();
            myCharge.Amount = amount;
            myCharge.Currency = "usd";
            myCharge.Description = "Bitcoin API Call";
            myCharge.SourceTokenOrExistingSourceId = token;
            myCharge.Capture = true;
            stripeChargeService.ApiKey = paymentSettings.StripePrivateKey;
            StripeCharge stripeCharge = stripeChargeService.Create(myCharge);

            if (String.IsNullOrEmpty(stripeCharge.FailureCode) && String.IsNullOrEmpty(stripeCharge.FailureMessage))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
