using System.ComponentModel.DataAnnotations;

namespace SEM.Models.ViewModels
{
    public class MemberProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [Display(Name = "Event Preferences")]
        public string? EventPreferences { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}
