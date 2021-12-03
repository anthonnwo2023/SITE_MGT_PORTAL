using Microsoft.AspNetCore.Identity;
using Project.V1.DLL.Helpers;
using Project.V1.Lib.Helpers;
using Project.V1.Models;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services.Login
{
    public class ExternalUserLogin : IUserLoginType
    {
        public async Task<SignInResponse> DoLogin(string username, string password, string vendorId)
        {
            Log.Logger = HelperFunctions.GetSerilogLogger();

            Log.Information("External login process. ", new { Username = username, Vendor = vendorId, UserType = "External" });
            VendorModel Vendor = (await LoginObject.Vendor.Get()).FirstOrDefault(x => x.Id == vendorId);

            ApplicationUser user = await LoginObject.User.GetUserByUsername(username.ToLower());

            if (user != null)
            {
                user.Roles = (await LoginObject.User.GetUserRoles(user)).ToArray();
                SignInResult result = await LoginObject.SignInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: true);
                LoginObject.ContextAccessor.HttpContext.User = await LoginObject.SignInManager.CreateUserPrincipalAsync(user);
                return await ProcessSignInResultOldUser(username, password, vendorId, Vendor, user, result);
            }

            return HelperLogin.ExtractResponse(null, SignInResult.Failed, "Invalid login attempt.");
        }

        private static async Task<SignInResponse> ProcessSignInResultOldUser(string username, string password, string vendorId, VendorModel Vendor, ApplicationUser user,
            SignInResult result)
        {

            if (user.Vendor.Id != vendorId)
            {
                return HelperLogin.ExtractResponse(null, SignInResult.Failed, "Invalid login attempt.");
            }

            return await HelperLogin.PerformSignInOp(username, password, vendorId, Vendor, user, result, null);
        }
    }
}