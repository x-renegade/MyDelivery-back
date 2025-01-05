﻿namespace Application.Common.Contracts.Repositories;

using Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByNameAsync(string userName);
    Task<User?> GetUserByIdAsync(string id);
    Task<IList<string>> GetUserRolesAsync(User user);
    Task<IList<User>> GetAllUsersAsync();
    Task<bool> CreateUserAsync(User user, string password);
    Task UpdateUserAsync(User user);
    Task AddUserToRoleAsync(User user, string role);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> SignInUserAsync(User user, bool isPersistent);
    Task SignOutUserAsync();
}
