using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SEM.Areas.Identity.Data
{
    public class SEMUser : IdentityUser
    {
        [PersonalData]
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [PersonalData]
        [Display(Name = "Bio")]
        [MaxLength(500)]
        public string? Bio { get; set; }

        [PersonalData]
        [Display(Name = "Event Preferences")]
        public string? EventPreferences { get; set; }


    }
}
