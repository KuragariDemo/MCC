using System.ComponentModel.DataAnnotations;

namespace MCC.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
