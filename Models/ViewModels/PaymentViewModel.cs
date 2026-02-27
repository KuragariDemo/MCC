using System.ComponentModel.DataAnnotations;

namespace MCC.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int EventId { get; set; }
        public string SeatType { get; set; }
        public decimal Price { get; set; }

        [Required]
        public string CardName { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        public string Expiry { get; set; }

        [Required]
        public string CVV { get; set; }
    }
}
