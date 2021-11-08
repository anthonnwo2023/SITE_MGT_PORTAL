using Project.V1.Models;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services.Login
{
    public interface IUserLogin
    {
        Task<SignInResponse> Login(string username, string password, string vendor);

        void ChangeLoginType(IUserLoginType loginType);
    }
}
