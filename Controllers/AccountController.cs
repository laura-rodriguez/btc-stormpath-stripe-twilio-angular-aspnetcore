using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using stormpath_angularjs_dotnet_stripe_twilio.Services;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace stormpath_angularjs_dotnet_stripe_twilio.Controllers
{
    
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }


        // GET: /<controller>/
        [Route("GetUserAccountInfo")]
        public async Task<IActionResult> GetUserAccountInfo()
        {
            var userAccountInfo = await _accountService.GetUserAccountInfo(HttpContext.User.Identity);

            return Ok(userAccountInfo);
        }

    }
}
