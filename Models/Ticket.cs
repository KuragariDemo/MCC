using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEM.Areas.Identity.Data;

namespace SEM.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public string UserId { get; set; }
        public SEMUser User { get; set; }

        public string SeatType { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}