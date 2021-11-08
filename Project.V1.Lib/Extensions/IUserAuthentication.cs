using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.Lib.Extensions
{
    public interface IUserAuthentication
    {
        Task<ClaimsPrincipal> GetLoggedInUser();
        Task<bool> IsAuthenticatedAsync();
        Task<bool> IsAuthenticatedCookieAsync();
        Task<bool> IsAutorizedForAsync(string PolicyName);
    }
}
