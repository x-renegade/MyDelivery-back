using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class Contex(DbContextOptions<Contex> options) : IdentityDbContext<User>(options)
    {
    }
}
