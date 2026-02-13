using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SEM.Areas.Identity.Data;
using SEM.Models;

namespace SEM.Data;

public class SEMContext : IdentityDbContext<SEMUser>
{
    public SEMContext(DbContextOptions<SEMContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }
}
