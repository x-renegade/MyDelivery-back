
using Application.Common.Contracts.Repositories;
using Application.Common.Exceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository(UserManager<User> userManager) : IUserRepository
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
        public async Task SignOutUserAsync(User user)
        {
            // Удаляем RefreshToken и его срок действия
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await UpdateUserAsync(user);
        }

        

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<bool> SignUpAsync(User user,string password)
        {
            var result = await CreateUserAsync(user, password);
            if (!result)
                throw new UserException("Failed to create user.");

            await AddUserToRoleAsync(user, UserRoles.User); 
            return result;
        }
    }
}
