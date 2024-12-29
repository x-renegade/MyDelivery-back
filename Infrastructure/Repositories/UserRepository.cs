
using Application.Common.Contracts.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository(UserManager<User> userManager, SignInManager<User> signInManager) : IUserRepository
    {
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<IList<User>> GetAllUsersAsync()
        {
            return await userManager.Users.ToListAsync();
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task UpdateUserAsync(User user)
        {
            await userManager.UpdateAsync(user);
        }

        public async Task AddUserToRoleAsync(User user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
        public async Task SignOutUserAsync()
        {
            // Получаем текущего пользователя из контекста
            var user = await userManager.GetUserAsync(signInManager.Context.User) ?? 
                throw new InvalidOperationException("No authenticated user found.");

            // Удаляем RefreshToken и его срок действия
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await UpdateUserAsync(user);

            // Завершаем сессию
            await signInManager.SignOutAsync();
        }

        public async Task<bool> SignInUserAsync(User user, bool isPersistent)
        {
            ArgumentNullException.ThrowIfNull(user);

            await signInManager.SignInAsync(user, isPersistent);
            return true;
        }
    }
}
