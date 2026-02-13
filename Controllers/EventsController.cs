using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEM.Data;
using SEM.Models;
using Microsoft.AspNetCore.Authorization;

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

        public async Task<IActionResult> Index(
            string category,
            string location,
            DateTime? date,
            decimal? maxPrice)
        {
            var events = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                events = events.Where(e => e.Category.Contains(category));

            if (!string.IsNullOrEmpty(location))
                events = events.Where(e => e.Location.Contains(location));

            if (date.HasValue)
                events = events.Where(e => e.Date.Date == date.Value.Date);

            if (maxPrice.HasValue)
                events = events.Where(e => e.Price <= maxPrice);

            return View(await events.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Buy(int id, string seatType)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
                return NotFound();

            if (seatType == "Low" && eventItem.LowSeats <= 0)
                return Content("No seats left.");

            if (seatType == "Medium" && eventItem.MediumSeats <= 0)
                return Content("No seats left.");

            if (seatType == "High" && eventItem.HighSeats <= 0)
                return Content("No seats left.");

            // Reduce seat
            if (seatType == "Low") eventItem.LowSeats--;
            if (seatType == "Medium") eventItem.MediumSeats--;
            if (seatType == "High") eventItem.HighSeats--;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }


    }

}
