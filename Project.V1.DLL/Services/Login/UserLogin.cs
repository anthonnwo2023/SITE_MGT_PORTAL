using Project.V1.Lib.Helpers;
using Project.V1.Models;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services.Login
{

    [SupportedOSPlatform("windows")]
    public class UserLogin : IUserLogin
    {
        private IUserLoginType _loginType;

        public UserLogin() : this(new InternalUserLogin()) { }

        public UserLogin(IUserLoginType loginType)
        {
            _loginType = loginType;
        }

        public void ChangeLoginType(IUserLoginType loginType)
        {
            _loginType = loginType;
        }

        public async Task<SignInResponse> Login(string username, string password, string vendor)
        {
            return await _loginType.DoLogin(username, password, vendor);
        }
    }
}
