using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCC.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        // ===== Seats & Pricing =====
        public decimal LowPrice { get; set; }
        public decimal MediumPrice { get; set; }
        public decimal HighPrice { get; set; }

        public int LowSeats { get; set; }
        public int MediumSeats { get; set; }
        public int HighSeats { get; set; }

        // ===== RELATIONSHIPS =====
        [Required(ErrorMessage = "Category is required")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public int? VenueId { get; set; }
        public Venue? Venue { get; set; }

    }
}
