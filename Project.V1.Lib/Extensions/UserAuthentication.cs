using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.Lib.Extensions
{
    public class UserAuthentication : IUserAuthentication
    {
        public IAuthorizationService AuthorizationService { get; }
        public IConfiguration Configuration { get; }
        public ClaimsPrincipal LoggedInUser { get; set; }

        public UserAuthentication(IAuthorizationService AuthorizationService, IConfiguration configuration)
        {
            HttpContextAccessor httpContextAccessor = new();
            LoggedInUser = httpContextAccessor.HttpContext.User;

            this.AuthorizationService = AuthorizationService;
            Configuration = configuration;
        }

        public async Task<ClaimsPrincipal> GetLoggedInUser()
        {
            return await Task.FromResult(LoggedInUser);
        }

        public async Task<bool> IsAuthenticatedCookieAsync()
        {
            return await Task.FromResult(LoggedInUser.Identity.IsAuthenticated);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await Task.FromResult(LoggedInUser.Identity.IsAuthenticated);
        }

        public async Task<bool> IsAutorizedForAsync(string PolicyName)
        {
            try
            {
                if (LoggedInUser.Identity.IsAuthenticated)
                {
                    AuthorizationResult AuthoriseUser = (await AuthorizationService.AuthorizeAsync(LoggedInUser, PolicyName));
                    return AuthoriseUser.Succeeded;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
