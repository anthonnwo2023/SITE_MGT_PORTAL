using System.Runtime.Versioning;

namespace Project.V1.Lib.Services.Login
{
    [SupportedOSPlatform("windows")]
    public class LoginFactory
    {
        public static IUserLogin Create(LoginTypes loginType)
        {
            return loginType switch
            {
                LoginTypes.MTN => new MTNStaffLogin(),
                LoginTypes.Vendor => new VendorUserLogin(),
                _ => null,
            };
        }
    }
}
