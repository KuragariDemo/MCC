using Microsoft.AspNetCore.Mvc.Rendering;
using SEM.Models;

namespace SEM.Models.ViewModels
{
    public class EventViewModel
    {
        public Event? Event { get; set; }

        public List<Event>? Events { get; set; }

        public SelectList? Categories { get; set; }

        public SelectList? Venues { get; set; }
    }
}
