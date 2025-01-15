
using Application.Common.Contracts.Repositories;
using Application.Common.Exceptions;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User;

public class UserRepository(UserManager<Shared.Identity.User> userManager) : IUserRepository
{
    public async Task<Shared.Identity.User?> GetUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<Shared.Identity.User?> GetUserByNameAsync(string userName)
    {
        return await userManager.FindByNameAsync(userName);
    }

    public async Task<IList<string>> GetUserRolesAsync(Shared.Identity.User user)
    {
        return await userManager.GetRolesAsync(user);
    }

    public async Task<IList<Shared.Identity.User>> GetAllUsersAsync()
    {
        return await userManager.Users.ToListAsync();
    }

    public async Task<bool> CreateUserAsync(Shared.Identity.User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task UpdateUserAsync(Shared.Identity.User user)
    {
        await userManager.UpdateAsync(user);
    }

    public async Task AddUserToRoleAsync(Shared.Identity.User user, string role)
    {
        await userManager.AddToRoleAsync(user, role);
    }

    public async Task<bool> CheckPasswordAsync(Shared.Identity.User user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }
    public async Task SignOutUserAsync(Shared.Identity.User user)
    {
        // Удаляем RefreshToken и его срок действия
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await UpdateUserAsync(user);
    }



    public async Task<Shared.Identity.User?> GetUserByIdAsync(string id)
    {
        return await userManager.FindByIdAsync(id);
    }

    public async Task<bool> SignUpAsync(Shared.Identity.User user, string password)
    {
        var result = await CreateUserAsync(user, password);
        if (!result)
            throw new UserException("Failed to create user.");

        await AddUserToRoleAsync(user, UserRoles.User);
        return result;
    }
}
