using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MCC.Areas.Identity.Data;
using MCC.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ use new connection string name
var connectionString = builder.Configuration
    .GetConnectionString("MCCContextConnection")
    ?? throw new InvalidOperationException("Connection string 'MCCContextConnection' not found.");

builder.Services.AddDbContext<MCCContext>(options =>
    options.UseSqlServer(connectionString));

// 🔐 Identity Configuration
builder.Services.AddIdentity<SEMUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<MCCContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


// 🔥 CREATE ROLES + DEFAULT ADMIN USER
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<SEMUser>>();

    string[] roles = { "Admin", "Member" };

    // 1️⃣ Create Roles
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 2️⃣ Create Default Admin User
    string adminEmail = "admin@mcc.com";   // ✅ updated
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new SEMUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator"
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

    var context = services.GetRequiredService<MCCContext>();

    var events = context.Events.ToList();

    foreach (var ev in events)
    {
        if (ev.Title == "Music Festival 2026")
        {
            ev.LowPrice = 1000;
            ev.MediumPrice = 1500;
            ev.HighPrice = 2500;

            ev.LowSeats = 100;
            ev.MediumSeats = 70;
            ev.HighSeats = 40;
        }

        if (ev.Title == "Tech Conference 2026")
        {
            ev.LowPrice = 2000;
            ev.MediumPrice = 3000;
            ev.HighPrice = 4500;

            ev.LowSeats = 150;
            ev.MediumSeats = 100;
            ev.HighSeats = 50;
        }
    }

    await context.SaveChangesAsync();
}

// ---------------- PIPELINE ----------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();