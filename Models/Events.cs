using System;
using System.ComponentModel.DataAnnotations;

namespace SEM.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public decimal LowPrice { get; set; }
        public decimal MediumPrice { get; set; }
        public decimal HighPrice { get; set; }

        public int LowSeats { get; set; }
        public int MediumSeats { get; set; }
        public int HighSeats { get; set; }

    }
}
