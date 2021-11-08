using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Services;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.DLL.Helpers
{
    public static class UserManagerExtension
    {
        private static async Task<bool> RemoveUserClaims(ApplicationUser user)
        {
            Log.Logger = HelperFunctions.GetSerilogLogger();

            try
            {
                var userClaims = await LoginObject.Context.UserClaims.Where(c => c.UserId == user.Id).ToListAsync();

                LoginObject.Context.UserClaims.RemoveRange(userClaims);

                await LoginObject.Context.SaveChangesAsync();

                //await userManager.RemoveClaimsAsync(user, userClaims);

                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public static async Task<bool> AddUserClaims(this UserManager<ApplicationUser> userManager, ApplicationUser user, List<ClaimViewModel> claims)
        {
            LoginObject.InitObjects();

            try
            {
                ApplicationUser userData = (await userManager.Users.AsNoTracking()
                    .Include(x => x.Regions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id));

                await RemoveUserClaims(userData);

                var userIdentityClaims = claims.Select(x => new IdentityUserClaim<string>
                {
                    UserId = userData.Id,
                    ClaimType = x.ClaimName,
                    ClaimValue = x.ClaimValue
                }).ToList();

                await LoginObject.Context.UserClaims.AddRangeAsync(userIdentityClaims);

                await LoginObject.Context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public static List<ClaimViewModel> FormatClaimSelection(this List<ClaimViewModel> allRoleClaims, ApplicationUser user)
        {
            LoginObject.InitObjects();

            IList<Claim> roleClaims = LoginObject.UserManager.GetClaimsAsync(user).GetAwaiter().GetResult();

            allRoleClaims.ForEach(item =>
            {
                item.IsSelected = false;
            });

            allRoleClaims.ForEach(allClaim =>
            {
                if (allRoleClaims.Count > 0 && roleClaims.Any(c => c.Type == allClaim.ClaimName))
                {
                    allClaim.IsSelected = true;
                }
            });

            return allRoleClaims;
        }
    }
}
