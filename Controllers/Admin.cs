using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEM.Areas.Identity.Data;
using SEM.Data;
using SEM.Models;
using SEM.Models.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly SEMContext _context;
    private readonly UserManager<SEMUser> _userManager;

    public AdminController(SEMContext context, UserManager<SEMUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    // ================= DASHBOARD =================

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.TotalEvents = await _context.Events.CountAsync();
        ViewBag.TotalUsers = await _userManager.Users.CountAsync();

        return View();
    }

    // =====================================================
    // ======================= EVENT ========================
    // =====================================================
    // GET
    public async Task<IActionResult> CreateEvent()
    {
        var vm = new EventViewModel
        {
            Event = new Event(),
            Events = await _context.Events
                        .Include(e => e.Category)
                        .Include(e => e.Venue)
                        .ToListAsync(),

            Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name"),
            Venues = new SelectList(await _context.Venues.ToListAsync(), "Id", "Name")
        };

        return View(vm);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(EventViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Events = await _context.Events
                        .Include(e => e.Category)
                        .Include(e => e.Venue)
                        .ToListAsync();

            vm.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            vm.Venues = new SelectList(await _context.Venues.ToListAsync(), "Id", "Name");

            return View(vm);
        }

        _context.Events.Add(vm.Event!);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Event Created Successfully!";

        return RedirectToAction(nameof(CreateEvent));
    }



    public async Task<IActionResult> EditEvent(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null)
            return NotFound();

        await LoadEventDropdowns(eventItem.CategoryId, eventItem.VenueId);

        return View(eventItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(Event model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEventDropdowns(model.CategoryId, model.VenueId);
            return View(model);
        }

        _context.Events.Update(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Event Updated Successfully!";
        return RedirectToAction(nameof(CreateEvent));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null)
            return NotFound();

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Event Deleted Successfully!";
        return RedirectToAction(nameof(CreateEvent));
    }

    // =====================================================
    // ======================= VENUE ========================
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> Venues()
    {
        var venues = await _context.Venues.ToListAsync();
        return View(venues);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Venues(Venue model)
    {
        if (!ModelState.IsValid)
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        _context.Venues.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Venue Added Successfully!";
        return RedirectToAction(nameof(Venues));
    }

    public async Task<IActionResult> EditVenue(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue == null)
            return NotFound();

        return View(venue);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditVenue(Venue model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.Venues.Update(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Venue Updated Successfully!";
        return RedirectToAction(nameof(Venues));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVenue(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        if (venue == null)
            return NotFound();

        if (await _context.Events.AnyAsync(e => e.VenueId == id))
        {
            TempData["Error"] = "Cannot delete venue because it is used by events.";
            return RedirectToAction(nameof(Venues));
        }

        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Venue Deleted Successfully!";
        return RedirectToAction(nameof(Venues));
    }

    // =====================================================
    // ===================== CATEGORY ======================
    // =====================================================

    public async Task<IActionResult> CreateCategory()
    {
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(Category model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(CreateCategory));

        _context.Categories.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Category Added Successfully!";
        return RedirectToAction(nameof(CreateCategory));
    }

    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(Category model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.Categories.Update(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Category Updated Successfully!";
        return RedirectToAction(nameof(CreateCategory));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        if (await _context.Events.AnyAsync(e => e.CategoryId == id))
        {
            TempData["Error"] = "Cannot delete category because it is used by events.";
            return RedirectToAction(nameof(CreateCategory));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Category Deleted Successfully!";
        return RedirectToAction(nameof(CreateCategory));
    }

    // ================= Helper =================
    private async Task LoadEventDropdowns(int? categoryId = null, int? venueId = null)
    {
        ViewBag.Categories = new SelectList(
            await _context.Categories.ToListAsync(),
            "Id",
            "Name",
            categoryId
        );

        ViewBag.Venues = new SelectList(
            await _context.Venues.ToListAsync(),
            "Id",
            "Name",
            venueId
        );
    }

    // ================= CREATE ADMIN =================
    public IActionResult CreateAdmin()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(string fullName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "All fields are required.";
            return View();
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            ViewBag.Error = "User already exists.";
            return View();
        }

        var user = new SEMUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            ViewBag.Success = "Admin created successfully!";
        }
        else
        {
            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
        }

        return View();
    }



}
