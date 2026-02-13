namespace SEM.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public string SeatType { get; set; }   // Low / Medium / High

        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        public decimal Price { get; set; }

        // Relationship
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
