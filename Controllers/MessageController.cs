using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using stormpath_angularjs_dotnet_stripe_twilio.Services;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using stormpath_angularjs_dotnet_stripe_twilio.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace stormpath_angularjs_dotnet_stripe_twilio.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly AccountService _accountService;
        private readonly SMSService _smsService;
        private readonly BitcoinExchangerRateService _bitcoinExchangerRateService;
        private readonly int FIXED_TOTAL_QUERY_INCREMENT = 1;

        public MessageController(AccountService accountService, SMSService smsService, BitcoinExchangerRateService bitcoinExchangerRateService)
        {
            _accountService = accountService;
            _smsService = smsService;
            _bitcoinExchangerRateService = bitcoinExchangerRateService;
        }        

        [HttpPost]        
        public async Task<IActionResult> Post([FromBody] SendSMSRequest payload)
        {
            if(string.IsNullOrEmpty(payload.PhoneNumber))
            {
                return BadRequest("Invalid phone number");
            }

            var userAccountInfo = await _accountService.GetUserAccountInfo(HttpContext.User.Identity);

            if (userAccountInfo.Balance == 0)
            {
                return StatusCode((int)HttpStatusCode.PaymentRequired);
            }

            try
            {
                var btcExchangeRate = await _bitcoinExchangerRateService.GetBitcoinExchangeRate();
                var message = $"1 Bitcoin is currently worth ${btcExchangeRate} USD.";

                await _smsService.SendSMS(message, payload.PhoneNumber);

                userAccountInfo = await _accountService.UpdateUserTotalQueries(HttpContext.User.Identity, FIXED_TOTAL_QUERY_INCREMENT);
                userAccountInfo = await _accountService.UpdateUserBalance(HttpContext.User.Identity, -PaymentService.FIXED_COST_PER_QUERY);

                return Ok(userAccountInfo);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
