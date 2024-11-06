using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;


namespace StudioManagement.Models
{
    public class User
    {
        [Key]
        [DisplayName("ID")]
        public int UserID { get; set; }
        [MaxLength(36)]
        [DisplayName("UUID")]
        public string? UserUUID { get; set; } = string.Empty;
        [MaxLength(45)]
        [DisplayName("User")]
        public string? FirstName { get; set; }
        [MaxLength(45)]
        [DisplayName("Full Name")]
        public string? LastName { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        public string? Password { get; set; }

        [NotMapped]
        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
        public string? FullName()
        {
            return this.FirstName + " " + this.LastName;
        }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }

        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
        //[ForeignKey("MemberId")]
        [ValidateNever]
        public Member? Member { get; set; }
        //[ForeignKey("EmployeeID")]
        [ValidateNever]
        public Employees? Employees { get; set; }
    }
}
