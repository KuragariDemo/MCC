using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEM.Areas.Identity.Data;
using SEM.Data;
using SEM.Models.ViewModels;
using System.Security.Claims;

namespace SEM.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly UserManager<SEMUser> _userManager;
        private readonly SignInManager<SEMUser> _signInManager;
        private readonly SEMContext _context;


        public MemberController(
            UserManager<SEMUser> userManager,
            SignInManager<SEMUser> signInManager,
            SEMContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // =========================
        // Dashboard
        // =========================
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // GET: Profile
        // =========================
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new MemberProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                EventPreferences = user.EventPreferences
            };

            return View(model);
        }

        // POST: Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(MemberProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;
            user.EventPreferences = model.EventPreferences;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Change Password (if entered)
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            ViewBag.StatusMessage = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }


        // =========================
        // Booked Tickets
        // =========================

        public async Task<IActionResult> MyTickets()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var tickets = await _context.Tickets
                .Include(t => t.Event)
                    .ThenInclude(e => e.Category)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Venue)
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return View(tickets);
        }

        // =========================
        // Ticket Event Details
        // =========================
        public async Task<IActionResult> MyTicketsDetails(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Check that ticket belongs to this user
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                    .ThenInclude(e => e.Category)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(t => t.EventId == id && t.UserId == userId);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }



    }
}
