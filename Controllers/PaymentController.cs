using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using stormpath_angularjs_dotnet_stripe_twilio.Services;
using stormpath_angularjs_dotnet_stripe_twilio.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace stormpath_angularjs_dotnet_stripe_twilio.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly AccountService _accountService;
        
        public PaymentController(PaymentService paymentService, AccountService accountService)
        {
            _paymentService = paymentService;
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentFormData formData)
        {
            if (_paymentService.ProcessPayment(formData.Token, PaymentService.FIXED_AMOUNT))
            {
                var userAccountInfo = await _accountService.UpdateUserBalance(HttpContext.User.Identity, PaymentService.FIXED_AMOUNT);
                return Ok(userAccountInfo);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
