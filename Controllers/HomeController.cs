using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEM.Data;
using SEM.Models;
using System.Diagnostics;

namespace SEM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SEMContext _context;   

        public HomeController(ILogger<HomeController> logger,
                              SEMContext context)  
        {
            _logger = logger;
            _context = context;   
        }

        public IActionResult Index()
        {
            var latestEvents = _context.Events
                .Include(e => e.Venue)     // ✅ include Venue
                .Include(e => e.Category)  // ✅ include Category (if you have it)
                .OrderByDescending(e => e.Id)
                .Take(3)
                .ToList();

            return View(latestEvents);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
