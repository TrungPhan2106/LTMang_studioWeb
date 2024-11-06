using System.ComponentModel.DataAnnotations;

namespace StudioManagement.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}