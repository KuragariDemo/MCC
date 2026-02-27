using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCC.Data;
using MCC.Models;
using MCC.Models.ViewModels;
using System.Security.Claims;

namespace MCC.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly MCCContext _context;

        public EventsController(MCCContext context)
        {
            _context = context;
        }

        #region ================= Event Listing & Filtering =================

        public IActionResult Index(string? category, string? location, DateTime? date, decimal? maxPrice)
        {
            var events = _context.Events
                                 .Include(e => e.Category)
                                 .Include(e => e.Venue)
                                 .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                events = events.Where(e => e.Category.Name.Contains(category));

            if (!string.IsNullOrWhiteSpace(location))
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

        #endregion


        #region ================= Event Details =================

        public IActionResult Details(int id)
        {
            var eventItem = _context.Events
                                    .Include(e => e.Category)
                                    .Include(e => e.Venue)
                                    .FirstOrDefault(e => e.Id == id);

            return eventItem == null ? NotFound() : View(eventItem);
        }

        #endregion


        #region ================= Buy Ticket =================

        // GET: Buy Ticket
        [HttpGet]
        public async Task<IActionResult> Buy(int id, string seatType)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            decimal price = seatType switch
            {
                "Low" => eventItem.LowPrice,
                "Medium" => eventItem.MediumPrice,
                "High" => eventItem.HighPrice,
                _ => 0
            };

            var vm = new PaymentViewModel
            {
                EventId = id,
                SeatType = seatType,
                Price = price
            };

            return View(vm);
        }

        // POST: Buy Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(PaymentViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var eventItem = await _context.Events.FindAsync(vm.EventId);
            if (eventItem == null) return NotFound();

            if (!HasAvailableSeat(eventItem, vm.SeatType))
                return Content("No seats left.");

            DecreaseSeat(eventItem, vm.SeatType);

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

        #endregion


        #region ================= Ticket Details =================

        public async Task<IActionResult> MyTicketsDetails(int id)
        {
            var ticket = await _context.Tickets
                                       .Include(t => t.Event)
                                       .FirstOrDefaultAsync(t => t.Id == id);

            return ticket == null ? NotFound() : View(ticket);
        }

        #endregion


        #region ================= Feedback =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int eventId, int rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Invalid rating value.";
                return RedirectToAction("MyTickets", "Member");
            }

            var hasTicket = await _context.Tickets
                                          .AnyAsync(t => t.EventId == eventId && t.UserId == userId);

            if (!hasTicket)
            {
                TempData["Error"] = "You cannot rate an event you didn't attend.";
                return RedirectToAction("MyTickets", "Member");
            }

            var eventItem = await _context.Events
                                          .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null || eventItem.Date >= DateTime.Now)
            {
                TempData["Error"] = "You can only rate finished events.";
                return RedirectToAction("MyTickets", "Member");
            }

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

        #endregion


        #region ================= Private Helpers =================

        private static bool HasAvailableSeat(Event eventItem, string seatType)
        {
            return seatType switch
            {
                "Low" => eventItem.LowSeats > 0,
                "Medium" => eventItem.MediumSeats > 0,
                "High" => eventItem.HighSeats > 0,
                _ => false
            };
        }

        private static void DecreaseSeat(Event eventItem, string seatType)
        {
            switch (seatType)
            {
                case "Low":
                    eventItem.LowSeats--;
                    break;
                case "Medium":
                    eventItem.MediumSeats--;
                    break;
                case "High":
                    eventItem.HighSeats--;
                    break;
            }
        }

        #endregion
    }
}