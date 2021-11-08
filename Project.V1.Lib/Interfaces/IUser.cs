using Project.V1.Data.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services.Interfaces
{
    public interface IUser : IGenericRepo<ApplicationUser>, IDisposable
    {
        Task<List<ApplicationUser>> GetUsers(bool isActive = true);
        Task<ApplicationUser> GetUserById(string UserId);
        Task<(bool, string)> CreateUser(ApplicationUser user, string Password, List<string> Roles);
        Task<bool> DeleteUser(ApplicationUser user);
        Task<bool> UpdateUser(ApplicationUser user, string Password, List<string> RoleId);
        Task<ApplicationUser> GetUserByUsername(string Username, bool isActive = true);
        Task ChangeUserRole(ApplicationUser user, string toRoleName, bool toAdd);
        Task<bool> IsUserInRole(ApplicationUser user, string roleName);
        Task ClearUserRoles(ApplicationUser user);
        Task<List<string>> GetUserRoles(ApplicationUser user);
        Task ChangeUserRoleByName(ApplicationUser user, string toRoleName, bool toAdd);
        Task<IEnumerable<ApplicationUser>> GetUsersInRole(string roleName);
        Task<List<string>> GetUserRolesId(ApplicationUser user);
        Task<List<Claim>> GetClaims(ApplicationUser user);
        Task<bool> AddClaim(ApplicationUser user, string type, string value);
        Task<bool> GenerateScope(ClaimsPrincipal principal);
        Task RemoveClaim(ClaimsPrincipal principal, string claimType);
    }
}
