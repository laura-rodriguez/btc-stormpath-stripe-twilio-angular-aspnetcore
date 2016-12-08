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
        private readonly IApplication stormpathApplication;
        private readonly string KEY_BALANCE = "Balance";
        private readonly string KEY_TOTAL_QUERIES = "TotalQueries";
        

        private async Task<IAccount> GetUserAccount(IIdentity userIdentity)
        {
            return await stormpathApplication.GetAccounts().Where(x => x.Email == userIdentity.Name).FirstOrDefaultAsync();
        }

        private async Task<UserAccountInfo> GetUserAccountInfo(IAccount userAccount)
        {
            UserAccountInfo userAccountInfo = new UserAccountInfo();
            var apiKeys = await userAccount.GetApiKeys().FirstAsync();
            var accountCustomData = await userAccount.GetCustomDataAsync();
            userAccountInfo.ApiKeyId = apiKeys.Id;
            userAccountInfo.ApiKeySecret = apiKeys.Secret;
            userAccountInfo.Balance = decimal.Parse(accountCustomData[KEY_BALANCE].ToString());
            userAccountInfo.TotalQueries = int.Parse(accountCustomData[KEY_TOTAL_QUERIES].ToString());

            return userAccountInfo;
        }

        public AccountService(IApplication stormpathApplication)
        {
            this.stormpathApplication = stormpathApplication;
        }

        public async Task<UserAccountInfo> GetUserAccountInfo(IIdentity userIdentity)
        {
            var userAccount = await GetUserAccount(userIdentity);
            return await GetUserAccountInfo(userAccount);
        }

        private async Task UpdateNumericCustomData(IAccount userAccount, decimal amount, string key)
        {
            var customData = await userAccount.GetCustomDataAsync();

            userAccount.CustomData[key] = Int64.Parse(customData[key].ToString()) + amount;
            await userAccount.SaveAsync();
        }

        public async Task<UserAccountInfo> UpdateUserBalance(IIdentity userIdentity, decimal amount)
        {
            var userAccount = await GetUserAccount(userIdentity);

            if (userAccount != null)
            {
                await UpdateNumericCustomData(userAccount, amount, KEY_BALANCE);
            }

            return await GetUserAccountInfo(userAccount);
        }

        public async Task<UserAccountInfo> UpdateUserTotalQueries(IIdentity userIdentity, int totalQueries)
        {
            var userAccount = await GetUserAccount(userIdentity);

            if (userAccount != null)
            {
                await UpdateNumericCustomData(userAccount, totalQueries, KEY_TOTAL_QUERIES);
            }

            return await GetUserAccountInfo(userAccount);
        }
    }
}
