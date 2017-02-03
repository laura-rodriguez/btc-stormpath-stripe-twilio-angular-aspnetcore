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
 
    [Authorize]   
    [Route("api/[controller]")]
    public class MeController : Controller
    {
        private readonly AccountService _accountService;

        public MeController(AccountService accountService)
        {
            _accountService = accountService;
        }


        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userAccountInfo = await _accountService.GetUserAccountInfo();

            return Ok(userAccountInfo);
        }

    }
}
