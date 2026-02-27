using MCC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MCC.Areas.Identity.Data;
using MCC.Models;

namespace MCC.Data;

public class MCCContext : IdentityDbContext<SEMUser>
{
    public MCCContext(DbContextOptions<MCCContext> options)
        : base(options)
    {
    }
    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }
}
