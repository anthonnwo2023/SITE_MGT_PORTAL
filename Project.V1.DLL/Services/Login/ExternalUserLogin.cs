using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.V1.DLL.Helpers;
using Project.V1.Lib.Helpers;
using Project.V1.Models;
using Serilog;
using System.Collections.Generic;
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
            VendorModel Vendor = (await LoginObject.Vendor.GetById(x => x.Id == vendorId));

            //ApplicationUser user = await LoginObject.User.GetUserByUsername(username.ToLower());
            ApplicationUser user = await LoginObject.UserManager.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());

            if (user != null)
            {
                user.Roles = (await LoginObject.User.GetUserRoles(user)).ToArray();
                List<ClaimListManager> userClaims = (LoginObject.ClaimService.Get(y => y.IsActive).GetAwaiter().GetResult()).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                {
                    Category = u.Key,
                    Claims = u.ToList().FormatClaimSelection(user)
                }).ToList();

                user.Projects = userClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();
                SignInResult result = await LoginObject.SignInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: true);
                return await ProcessSignInResultOldUser(username, vendorId, Vendor, user, result);
            }

            return HelperLogin.ExtractResponse(null, SignInResult.Failed, "Invalid login attempt.");
        }

        private static async Task<SignInResponse> ProcessSignInResultOldUser(string username, string vendorId, VendorModel Vendor, ApplicationUser user,
            SignInResult result)
        {

            if (user.Vendor.Id != vendorId)
            {
                return HelperLogin.ExtractResponse(null, SignInResult.Failed, "Invalid login attempt.");
            }

            return await HelperLogin.PerformSignInOp(username, vendorId, Vendor, user, result, null);
        }
    }
}