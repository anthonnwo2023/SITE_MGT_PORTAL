using Project.V1.Lib.Helpers;
using Project.V1.Models;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services.Login
{
    public interface IUserLoginType
    {
        Task<SignInResponse> DoLogin(string username, string password, string vendor);
    }
}