using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Lib.Extensions
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        //private readonly IConfiguration _configuration;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options/*, IConfiguration configuration*/) : base(options)
        {
            _options = options.Value;
            //_configuration = configuration;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            AuthorizationPolicy policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    //.AddRequirements(new HasScopeRequirement(policyName, $"https://{_configuration["Auth0:Domain"]}/"))
                    //.AddRequirements(new HasScopeRequirement(policyName, $"https://ojtssapp1/smp"))
                    .AddRequirements(new HasScopeRequirement(policyName, $"LOCAL AUTHORITY"))
                    .Build();

                // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
                _options.AddPolicy(policyName, policy);
            }

            return policy;
        }
    }

    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }

        public HasScopeRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }

    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            //LoginObject.InitObjects();

            //LoginObject.User.GenerateScope(context.User);

            //var claims = LoginObject.UserManager.GetClaimsAsync(LoginObject.UserManager.FindByNameAsync(context.User.Identity.Name).GetAwaiter().GetResult()).GetAwaiter().GetResult();
            // If user does not have the scope claim, get out of here
            //if ((!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer)) && !claims.Any(c => c.Type == "scope"))
            //    return Task.CompletedTask;

            // Split the scopes string into an array
            //var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer)?.Value?.Split('~')?.Distinct().Where(x => !x.Contains("scope"));

            //scopes = claims?.Where(x => x.Type == "scope").Select(x => x.Value).First()?.Split('~')?.Distinct().Where(x => !x.Contains("scope"));

            //// Succeed if the scope array contains the required scope
            //if (scopes.Any(s => s.Contains(requirement.Scope)))
            //    context.Succeed(requirement);

            if (context.User.Claims.Any(x => x.Type == requirement.Scope))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
