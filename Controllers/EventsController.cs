using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEM.Data;
using SEM.Models;
using SEM.Models.ViewModels;
using System.Security.Claims;

namespace SEM.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly SEMContext _context;

        public EventsController(SEMContext context)
        {
            _context = context;
        }

        // =====================================
        // EVENT LIST + FILTER
        // =====================================
        public IActionResult Index(string category, string location, DateTime? date, decimal? maxPrice)
        {
            var events = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                events = events.Where(e => e.Category.Name.Contains(category));

            if (!string.IsNullOrEmpty(location))
                events = events.Where(e => e.Venue.Location.Contains(location));

            if (date.HasValue)
                events = events.Where(e => e.Date.Date == date.Value.Date);

            if (maxPrice.HasValue)
            {
                events = events.Where(e =>
                    e.LowPrice <= maxPrice.Value ||
                    e.MediumPrice <= maxPrice.Value ||
                    e.HighPrice <= maxPrice.Value);
            }

            return View(events.ToList());
        }

        // =====================================
        // EVENT DETAILS
        // =====================================
        public IActionResult Details(int id)
        {
            var eventItem = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        // =====================================
        // BUY TICKET (GET)
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Buy(int id, string seatType)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound();

            decimal price = 0;

            if (seatType == "Low") price = eventItem.LowPrice;
            if (seatType == "Medium") price = eventItem.MediumPrice;
            if (seatType == "High") price = eventItem.HighPrice;

            var vm = new PaymentViewModel
            {
                EventId = id,
                SeatType = seatType,
                Price = price
            };

            return View(vm);
        }

        // =====================================
        // BUY TICKET (POST)
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(PaymentViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var eventItem = await _context.Events.FindAsync(vm.EventId);
            if (eventItem == null)
                return NotFound();

            if (vm.SeatType == "Low" && eventItem.LowSeats <= 0)
                return Content("No seats left.");

            if (vm.SeatType == "Medium" && eventItem.MediumSeats <= 0)
                return Content("No seats left.");

            if (vm.SeatType == "High" && eventItem.HighSeats <= 0)
                return Content("No seats left.");

            if (vm.SeatType == "Low") eventItem.LowSeats--;
            if (vm.SeatType == "Medium") eventItem.MediumSeats--;
            if (vm.SeatType == "High") eventItem.HighSeats--;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = new Ticket
            {
                EventId = vm.EventId,
                UserId = userId,
                SeatType = vm.SeatType,
                Price = vm.Price
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment Successful! Ticket Purchased.";

            return RedirectToAction("MyTickets", "Member");
        }

        // =====================================
        // MY TICKET DETAILS
        // =====================================
        public async Task<IActionResult> MyTicketsDetails(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        // =====================================
        // SUBMIT FEEDBACK
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int eventId, int rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate rating
            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Invalid rating value.";
                return RedirectToAction("MyTickets", "Member");
            }

            // Check user bought ticket
            var hasTicket = await _context.Tickets
                .AnyAsync(t => t.EventId == eventId && t.UserId == userId);

            if (!hasTicket)
            {
                TempData["Error"] = "You cannot rate an event you didn't attend.";
                return RedirectToAction("MyTickets", "Member");
            }

            // Ensure event is finished
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null || eventItem.Date >= DateTime.Now)
            {
                TempData["Error"] = "You can only rate finished events.";
                return RedirectToAction("MyTickets", "Member");
            }

            // Prevent duplicate rating
            var existing = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.EventId == eventId && f.UserId == userId);

            if (existing != null)
            {
                TempData["Error"] = "You already submitted feedback.";
                return RedirectToAction("MyTickets", "Member");
            }

            var feedback = new Feedback
            {
                EventId = eventId,
                UserId = userId,
                Rating = rating
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thank you for your feedback!";

            return RedirectToAction("MyTicketsDetails", new { id = eventId });
        }
    }
}