using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCC.Data;
using MCC.Models;
using System.Diagnostics;

namespace MCC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MCCContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            MCCContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region ================= Home =================

        public IActionResult Index()
        {
            var latestEvents = _context.Events
                                       .Include(e => e.Venue)
                                       .Include(e => e.Category)
                                       .OrderByDescending(e => e.Id)
                                       .Take(3)
                                       .ToList();

            return View(latestEvents);
        }

        #endregion


        #region ================= Privacy =================

        public IActionResult Privacy()
            => View();

        #endregion


        #region ================= Error Handling =================

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(model);
        }

        #endregion
    }
}