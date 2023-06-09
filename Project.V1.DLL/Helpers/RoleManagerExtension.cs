﻿using Microsoft.AspNetCore.Identity;
using Project.V1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.DLL.Helpers
{
    public static class RoleManagerExtension
    {
        public static async Task<bool> RemoveRoleClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            try
            {
                IList<Claim> roleClaims = await roleManager.GetClaimsAsync(role);

                foreach (Claim claim in roleClaims)
                {
                    await roleManager.RemoveClaimAsync(role, claim);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> AddRoleClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, List<ClaimViewModel> claims)
        {
            try
            {
                await roleManager.RemoveRoleClaims(role);

                foreach (ClaimViewModel claim in claims)
                {
                    Claim newClaim = new(claim.Name, claim.Value);

                    await roleManager.AddClaimAsync(role, newClaim);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<ClaimViewModel> FormatClaimSelection(this List<ClaimViewModel> allRoleClaims, IdentityRole role)
        {
            IList<Claim> roleClaims = LoginObject.RoleManager.GetClaimsAsync(role).GetAwaiter().GetResult();

            allRoleClaims.ForEach(item =>
            {
                item.IsSelected = false;
            });

            allRoleClaims.ForEach(allClaim =>
            {
                if (allRoleClaims.Count > 0 && roleClaims.Any(c => c.Type == allClaim.Name))
                {
                    allClaim.IsSelected = true;
                }
            });

            return allRoleClaims;
        }
    }
}
