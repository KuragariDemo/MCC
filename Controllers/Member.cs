using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SEM.Areas.Identity.Data;
using SEM.Models.ViewModels;

namespace SEM.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly UserManager<SEMUser> _userManager;
        private readonly SignInManager<SEMUser> _signInManager;

        public MemberController(
            UserManager<SEMUser> userManager,
            SignInManager<SEMUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public IActionResult BookedTickets()
        {
            return View();
        }
    }
}
