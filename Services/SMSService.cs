using Microsoft.Extensions.Options;
using stormpath_angularjs_dotnet_stripe_twilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace stormpath_angularjs_dotnet_stripe_twilio.Services
{
    public class SMSService
    {
        private readonly SMSSettings _smsSettings;

        public SMSService(IOptions<SMSSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }

        public async Task SendSMS(string message, string phoneNumber)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(_smsSettings.BaseUri) })
            {
                phoneNumber = phoneNumber.Trim();

                if (phoneNumber.StartsWith("+"))
                {
                    phoneNumber = phoneNumber.Substring(1);
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_smsSettings.Sid}:{_smsSettings.Token}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", $"+{phoneNumber}"),
                    new KeyValuePair<string, string>("From", _smsSettings.From),
                    new KeyValuePair<string, string>("Body", message)
                });

                var response = await client.PostAsync(_smsSettings.RequestUri, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("An error occurred while sending the SMS");
                }
            }
       }
    }
}
