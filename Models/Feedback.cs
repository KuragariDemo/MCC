using System;
using System.ComponentModel.DataAnnotations;

namespace MCC.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public string UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}