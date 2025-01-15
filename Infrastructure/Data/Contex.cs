
using Domain.Entities.Map;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Identity;

namespace Infrastructure.Data;

public class Contex(DbContextOptions<Contex> options) : IdentityDbContext<User>(options)
{
    public DbSet<Location> Locations { get; set; } = null!;
}
