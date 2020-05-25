using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MiApp.Web.Data.Entities;
using MiApp.Web.Models;
using System;
using MiApp.Common.Enums;

namespace MiApp.Web.Helpers
{
    public interface IUserHelper
    {
        Task<UserEntity> GetUserAsync(string email);
        Task<UserEntity> GetUserAsync(Guid userId);

        Task<IdentityResult> AddUserAsync(UserEntity user, string password);
        

        Task<UserEntity> AddUserAsync(AddUserViewModel model, string path);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(UserEntity user, string roleName);

        Task<bool> IsUserInRoleAsync(UserEntity user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();


        Task<string> GenerateEmailConfirmationTokenAsync(UserEntity user);

        Task<IdentityResult> ConfirmEmailAsync(UserEntity user, string token);

        Task<UserEntity> AddUserAsync(AddUserViewModel model, string path, UserType userType);

        Task<IdentityResult> UpdateUserAsync(UserEntity user);

        Task<SignInResult> ValidatePasswordAsync(UserEntity user, string password);

        Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword);
        
        Task<string> GeneratePasswordResetTokenAsync(UserEntity user);
        
        Task<IdentityResult> ResetPasswordAsync(UserEntity user, string token, string password);

        Task<bool> DeleteUserAsync(string email);

    }
}