﻿namespace Project.V1.DLL.Helpers;

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

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return false;
        }
    }

    public static async Task<bool> AddUserClaims(this UserManager<ApplicationUser> userManager, ApplicationUser user, List<ClaimViewModel> claims)
    {
        try
        {
            await RemoveUserClaims(user);

            var userIdentityClaims = claims.Select(x => new IdentityUserClaim<string>
            {
                UserId = user.Id,
                ClaimType = x.Name,
                ClaimValue = x.Value
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
        try
        {
            if (LoginObject.UserManager == null)
                LoginObject.InitObjects();

            IList<Claim> roleClaims = LoginObject.UserManager.GetClaimsAsync(user).GetAwaiter().GetResult();

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
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new List<ClaimViewModel>();
        }
    }
}
