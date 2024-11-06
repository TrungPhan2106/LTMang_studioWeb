using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudioManagement.Models
{
    public class Employees
    {
        [Key]
        [DisplayName("ID")]
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [Required]
        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [DisplayName("Phone")]
        public string? PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public string? ImageUrl { get; set; }
        [DisplayName("Studio")]
        public int? StudioID { get; set; }
        [ForeignKey("StudioID")]
        [ValidateNever]
        public Studio? Studio { get; set; }
    }
}
