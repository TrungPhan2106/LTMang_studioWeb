using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;


namespace StudioManagement.Models
{
    public class Member
    {
        [Key]
        [DisplayName("ID")]
        public int MemberId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required]
        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get ; set; }
        [DisplayName("Phone")]
        public string? PhoneNumber { get; set; }
        public bool  Gender { get; set; }
        [MaxLength(50)] 
        public string? Address { get; set; }
        [DisplayName("Joined Date")]
        [DataType(DataType.Date)]
        public DateTime JoinedDate { get; set; }
        [DisplayName("Avatar")]
        public string? ImageUrl { get; set; }
        [DisplayName("Studio")]
        public int? StudioID { get; set; }
        [ForeignKey("StudioID")]
        [ValidateNever]
        public Studio? Studio { get; set; }
    }
}
