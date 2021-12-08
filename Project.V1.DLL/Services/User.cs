using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Project.V1.Data;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services
{
    public class User : GenericRepo<ApplicationUser>, IUser, IDisposable
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRegion _region;
        private readonly IVendor _vendor;
        private readonly ICLogger _logger;

        public User(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IVendor vendor, ICLogger logger,
            SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, IRegion region)
            : base(context, null)
        {
            _signInManager = signInManager;
            _region = region;
            _userManager = userManager;
            _roleManager = roleManager;
            _vendor = vendor;
            _logger = logger;
        }

        public async Task<bool> GenerateScope(ClaimsPrincipal principal)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(principal);

                await RemoveClaim(principal, "scope");

                bool scopeAdded = await AddClaim(user, "scope", string.Join('~', principal.Claims));

                if (scopeAdded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return false;
            }
        }

        public async Task RemoveClaim(ClaimsPrincipal principal, string claimType)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(principal);
                bool userHasClaim = principal.HasClaim(x => x.Type == claimType);

                if (userHasClaim)
                {
                    Claim claim = (await _userManager.GetClaimsAsync(user)).ToList().FirstOrDefault(x => x.Type == claimType);
                    claim = principal.FindFirst(claimType);
                    await _userManager.RemoveClaimAsync(user, claim);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return;
            }
        }

        public async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            try
            {
                return (await _userManager.GetClaimsAsync(user)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return default;
            }
        }

        public async Task<bool> AddClaim(ApplicationUser user, string type, string value)
        {
            try
            {
                Claim claim = new(type, value);

                await _userManager.AddClaimAsync(user, claim);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return false;
            }
        }

        public async Task<(bool, string)> CreateUser(ApplicationUser user, string Password, List<string> RoleIds)
        {
            try
            {
                ApplicationUser NewUser = new();
                ApplicationUser userExists = await _userManager.Users.Include(x => x.Regions).FirstOrDefaultAsync(x => x.UserName == user.UserName);

                List<VendorModel> Vendors = await _vendor.Get();
                VendorModel MTN_Vendor = Vendors.FirstOrDefault(x => x.Name.ToUpper() == "MTN NIGERIA");
                VendorModel userVendor = Vendors.FirstOrDefault(x => x.Id == user.VendorId);


                if (userExists != null)
                {
                    throw new Exception($"A user with username {user.UserName} exists.");
                }

                List<RegionViewModel> regions = new();

                NewUser = user;
                NewUser.UserName = user.UserName.ToLower();
                NewUser.DateCreated = DateTimeOffset.UtcNow.DateTime;
                NewUser.LastLoginDate = DateTimeOffset.UtcNow.DateTime;
                NewUser.IsActive = true;
                NewUser.UserType = (userVendor.Id == MTN_Vendor.Id) ? WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("Internal")) : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("External"));
                //NewUser.UserType = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("External"));

                foreach (var region in user.Regions)
                {
                    regions.Add(_region.GetById(x => x.Id == region.Id).Result);
                }

                NewUser.Regions = new List<RegionViewModel>();
                NewUser.Regions.AddRange(regions);

                IdentityResult result = (Password != null) ? _userManager.CreateAsync(NewUser, Password).Result : _userManager.CreateAsync(NewUser, "0004Fa1c-H!!3f7-47f4-9e58-25c9001d5426").Result;

                if (result.Succeeded)
                {
                    foreach (string roleId in RoleIds)
                    {
                        var roleName = (_roleManager.FindByIdAsync(roleId).Result).Name;

                        await _userManager.AddToRoleAsync(user, roleName);
                    }
                }

                return (true, NewUser.Id);
            }
            catch
            {
                return (false, null);
            }
        }

        public async Task<bool> DeleteUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new Exception($"A user with username cannot be found.");
            }
            else
            {
                ApplicationUser userFound = await _userManager.Users.Include(x => x.Regions).FirstOrDefaultAsync(x => x.Id == user.Id);

                userFound.IsActive = false;

                IdentityResult result = await _userManager.UpdateAsync(userFound);

                if (result.Succeeded)
                {
                    return true;
                }

                return false;
            }
        }

        public async Task<ApplicationUser> GetUserByUsername(string Username, bool isActive = true)
        {
            try
            {
                if (isActive == false)
                {
                    return await _userManager.Users.Include(x => x.Regions).FirstOrDefaultAsync(x => x.UserName.ToLower() == Username.ToLower());
                }

                return await _userManager.Users.Include(x => x.Regions).Where(x => x.IsActive == true).FirstOrDefaultAsync(x => x.UserName.ToLower() == Username.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
                return new ApplicationUser();
            }            
        }

        public async Task<ApplicationUser> GetUserById(string UserId)
        {
            return await _userManager.Users.Include(x => x.Regions).Where(x => x.IsActive == true).FirstOrDefaultAsync(x => x.Id == UserId);
        }

        public async Task<List<ApplicationUser>> GetUsers(bool isActive = true)
        {
            try
            {
                List<ApplicationUser> users = await _userManager.Users.Include(x => x.Regions).Where(x => x.IsActive == true).ToListAsync();
                if (isActive == false)
                {
                    users = await _userManager.Users.Include(x => x.Regions).ToListAsync();
                }

                users.ForEach(async (user) =>
                {
                    user.Roles = (await _userManager.GetRolesAsync(user)).ToArray();
                });

                return users;
            }
            catch
            {
                return null;
            }
        }

        private bool UserDataExists(string id)
        {
            return _userManager.Users.Include(x => x.Regions).Any(e => e.Id == id);
        }

        public async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<List<string>> GetUserRolesId(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            return roles.ToList().Select(x => (_roleManager.FindByNameAsync(x)).Result.Id).ToList();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRole(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task ClearUserRoles(ApplicationUser user)
        {
            foreach (IdentityRole role in _roleManager.Roles.ToList())
            {
                if (await IsUserInRole(user, role.Name))
                {
                    await ChangeUserRole(user, role.Id, false);
                }
            }
        }

        public async Task<bool> IsUserInRole(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task ChangeUserRole(ApplicationUser user, string toRoleId, bool toAdd)
        {
            ApplicationUser userExists = await _userManager.Users.Include(x => x.Regions).FirstOrDefaultAsync(x => x.Id == user.Id);
            IdentityRole role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == toRoleId);

            if (toAdd)
            {
                await _userManager.AddToRoleAsync(userExists, role.Name);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(userExists, role.Name);
            }
        }

        public async Task ChangeUserRoleByName(ApplicationUser user, string toRoleName, bool toAdd)
        {
            ApplicationUser userExists = await _userManager.Users.Include(x => x.Regions).FirstOrDefaultAsync(x => x.Id == user.Id);
            IdentityRole role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == toRoleName);

            if (toAdd)
            {
                await _userManager.AddToRoleAsync(userExists, role.Name);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(userExists, role.Name);
            }
        }

        public async Task<bool> UpdateUser(ApplicationUser user, string Password, List<string> RoleIds)
        {
            try
            {
                ApplicationUser userExists = await _userManager.Users.Include(x => x.Regions).FirstOrDefaultAsync(x => x.Id == user.Id);

                if (userExists == null)
                {
                    throw new Exception($"User with Id = {user.Id} cannot be found. Update Failed");
                }
                else
                {
                    List<RegionViewModel> regions = new();

                    userExists.Email = user.Email;
                    userExists.UserName = user.UserName.ToLower();
                    userExists.Fullname = user.Fullname;
                    userExists.PhoneNumber = user.PhoneNumber;
                    userExists.JobTitle = user.JobTitle;
                    userExists.Department = user.Department;
                    userExists.IsActive = user.IsActive;
                    userExists.VendorId = user.VendorId;

                    foreach (var region in user.Regions)
                    {
                        regions.Add(await _region.GetById(x => x.Id == region.Id));
                    }

                    userExists.Regions = new List<RegionViewModel>();

                    IdentityResult result = await _userManager.UpdateAsync(userExists);

                    userExists.Regions.AddRange(regions);
                    //userExists.Regions = user.Regions;

                    result = await _userManager.UpdateAsync(userExists);

                    if (Password != null)
                    {
                        //await _userManager.RemovePasswordAsync(userExists);
                        //await _userManager.AddPasswordAsync(userExists, Password);

                        string token = await _userManager.GeneratePasswordResetTokenAsync(userExists);
                        IdentityResult resultChangedPW = await _userManager.ResetPasswordAsync(userExists, token, Password);
                    }

                    if (result.Succeeded)
                    {
                        foreach (IdentityRole IdenRole in _roleManager.Roles)
                        {
                            await _userManager.RemoveFromRoleAsync(userExists, IdenRole.Name);
                        }

                        foreach (string roleId in RoleIds)
                        {
                            await _userManager.AddToRoleAsync(userExists, (await _roleManager.FindByIdAsync(roleId)).Name);
                        }
                    }
                    else
                    {
                        if (!UserDataExists(userExists.Id))
                        {
                            throw new Exception("User not found");
                        }
                        else
                        {
                            throw new Exception("Unknown Error");
                        }
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
