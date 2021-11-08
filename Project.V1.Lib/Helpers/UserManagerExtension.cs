using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.V1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.Lib.Helpers
{
    public static class UserManagerExtension
    {
        public static async Task<bool> RemoveUserClaims(this UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            try
            {
                ApplicationUser userData = await userManager.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                IList<Claim> userClaims = await userManager.GetClaimsAsync(userData);

                await userManager.RemoveClaimsAsync(userData, userClaims);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> AddUserClaims(this UserManager<ApplicationUser> userManager, ApplicationUser user, List<ClaimViewModel> claims)
        {
            try
            {
                await userManager.RemoveUserClaims(user);
                ApplicationUser userData = await userManager.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                foreach (ClaimViewModel claim in claims)
                {
                    Claim newClaim = new Claim(claim.ClaimName, claim.ClaimValue);

                    await userManager.AddClaimAsync(userData, newClaim);
                }

                return true;
            }
            catch
            {
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
