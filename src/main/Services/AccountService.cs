using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Main.Models;
using Main.ViewModels;

namespace Main.Services
{
    public class AccountService : IAccountService
    {
        private readonly IItemRepository _repository;
        private readonly IJwtHandler _jwtHandler;
        private readonly IPasswordHasher<UserManage> _passwordHasher;
        private readonly SignInManager<UserManage> _signInManager;
        private readonly UserManager<UserManage> _userManager;
        public AccountService(SignInManager<UserManage> signInManager, UserManager<UserManage> userM, IJwtHandler jwtHandler,
            IPasswordHasher<UserManage> passwordHasher, IItemRepository repository)
        {
            _repository = repository;
            _signInManager = signInManager;
            _jwtHandler = jwtHandler;
            _passwordHasher = passwordHasher;
            _userManager = userM;
        }


        public async Task<JsonWebToken> SignIn(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                if (signInResult.Succeeded)
                {
                    var jwt = _jwtHandler.Create(user);
                    var refreshToken = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                        .Replace("+", string.Empty)
                        .Replace("=", string.Empty)
                        .Replace("/", string.Empty);
                    jwt.RefreshToken = refreshToken;
                    user.RefreshToken = refreshToken;
                    _repository.UpdateRefreshToken(user);
                    return jwt;
                }
            }
            return null;
        }

        public async Task<JsonWebToken> RefreshAccessToken(string token)
        {
            var refreshToken = await GetRefreshToken(token).ConfigureAwait(false);
            if (refreshToken == null)
            {
                return null;
            }

            var jwt = _jwtHandler.Create(refreshToken); 
            jwt.RefreshToken = refreshToken.RefreshToken;

            return jwt;
        }



        private async Task<UserManage> GetRefreshToken(string token)
        {
            return _repository.GetUserManageWithRefreshToken(token);
        }

        public async Task<bool> AddUser(UserViewModel vm)
        {
            if (await this._userManager.FindByNameAsync(vm.Username) == null)
            {
                var user = new UserManage
                {
                    UserName = vm.Username,
                    Name = vm.Name,
                    LastName = vm.LastName,
                    Email = vm.Email,
                    Admin = vm.Admin
                };
               await this._userManager.CreateAsync(user, vm.Password);
               return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<IdentityResult> ResetUser(UserManage vm)
        {

            UserManage user = await this._userManager.FindByNameAsync(vm.UserName);
            string passwordResetTokenAsync = await this._userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult identityResult = await this._userManager.ResetPasswordAsync(user, passwordResetTokenAsync, "P@ssw0rd");
            return identityResult;
        }

        public async Task<IdentityResult> ChangePassword(UserManage vm, string oldPassword, string newPassword)
        {
            var re = this._userManager.CheckPasswordAsync(vm, oldPassword);
            if (re.Result)
            {
                IdentityResult identityResult = await this._userManager.ChangePasswordAsync(vm, oldPassword, newPassword);
                return identityResult;
            }
            else
            {
                return null;
            }
                  
        }
    }
}
