using Final_project.Entities;
using Final_project.Models;
using Microsoft.AspNetCore.Identity;

namespace Final_project.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByNameAsync(string username);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> ChangeUserPasswordAsync(User user, string currentPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task SignOutAsync();
    }
}
