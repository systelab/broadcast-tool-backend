using Main.Models;
using Main.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Main.Services
{
    public interface IAccountService
    {
        Task<JsonWebToken> SignIn(string username, string password);
        Task<JsonWebToken> RefreshAccessToken(string token);
        Task<bool> AddUser(UserViewModel vm);
        Task<IdentityResult> ResetUser(UserManage vm);
        Task<IdentityResult> ChangePassword(UserManage vm,string oldPassword, string  newPassword);
    }
}
