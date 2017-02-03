using Stormpath.SDK;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using stormpath_angularjs_dotnet_stripe_twilio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace stormpath_angularjs_dotnet_stripe_twilio.Services
{
    public class AccountService
    {
        private readonly IApplication _stormpathApplication;
        private readonly IAccount _account;
        public static readonly string KEY_BALANCE = "Balance";
        public static readonly string KEY_TOTAL_QUERIES = "TotalQueries";

        public AccountService(IAccount account, IApplication stormpathApplication)
        {
            _account = account;
            _stormpathApplication = stormpathApplication;
        }
        
        public async Task<UserAccountInfo> GetUserAccountInfo()
        {
            var userAccountInfo = new UserAccountInfo();
            var apiKeys = await _account.GetApiKeys().FirstAsync();
            var accountCustomData = await _account.GetCustomDataAsync();
            userAccountInfo.ApiKeyId = apiKeys.Id;
            userAccountInfo.ApiKeySecret = apiKeys.Secret;
            userAccountInfo.Balance = decimal.Parse(accountCustomData[KEY_BALANCE].ToString());
            userAccountInfo.TotalQueries = int.Parse(accountCustomData[KEY_TOTAL_QUERIES].ToString());

            return userAccountInfo;
        }

        private async Task UpdateNumericCustomData(decimal amount, string key)
        {
            var customData = await _account.GetCustomDataAsync();

            _account.CustomData[key] = Int64.Parse(customData[key].ToString()) + amount;
            await _account.SaveAsync();
        }

        public async Task<UserAccountInfo> UpdateUserBalance(decimal amount)
        {
            await UpdateNumericCustomData(amount, KEY_BALANCE);
           
            return await GetUserAccountInfo();
        }

        public async Task<UserAccountInfo> UpdateUserTotalQueries(int totalQueries)
        {
            await UpdateNumericCustomData(totalQueries, KEY_TOTAL_QUERIES);
            
            return await GetUserAccountInfo();
        }
    }
}
