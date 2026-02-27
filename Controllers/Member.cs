using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCC.Areas.Identity.Data;
using MCC.Data;
using MCC.Models.ViewModels;
using System.Security.Claims;

namespace MCC.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly UserManager<SEMUser> _userManager;
        private readonly SignInManager<SEMUser> _signInManager;
        private readonly MCCContext _context;

        public MemberController(
            UserManager<SEMUser> userManager,
            SignInManager<SEMUser> signInManager,
            MCCContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        #region ================= Dashboard =================

        public IActionResult Index()
            => View();

        #endregion


        #region ================= Profile =================

        // GET: Profile
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
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            // Change password (if provided)
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return View(model);
                }
            }

            await _signInManager.RefreshSignInAsync(user);

            ViewBag.StatusMessage = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        #endregion


        #region ================= Tickets =================

        public async Task<IActionResult> MyTickets()
        {
            var userId = GetCurrentUserId();

            var tickets = await _context.Tickets
                                        .Include(t => t.Event)
                                            .ThenInclude(e => e.Category)
                                        .Include(t => t.Event)
                                            .ThenInclude(e => e.Venue)
                                        .Where(t => t.UserId == userId)
                                        .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> MyTicketsDetails(int id)
        {
            var userId = GetCurrentUserId();

            var ticket = await _context.Tickets
                                       .Include(t => t.Event)
                                           .ThenInclude(e => e.Category)
                                       .Include(t => t.Event)
                                           .ThenInclude(e => e.Venue)
                                       .FirstOrDefaultAsync(t =>
                                           t.EventId == id &&
                                           t.UserId == userId);

            return ticket == null ? NotFound() : View(ticket);
        }

        #endregion


        #region ================= Helpers =================

        private string? GetCurrentUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);

        #endregion
    }
}