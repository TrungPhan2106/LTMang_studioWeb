using MemberManagement.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;


namespace MemberManagement.Data
{
    public class Member
    {
        [Key]
        [DisplayName("ID")]
        public int MemberId { get; set; }
        [MaxLength(36)]
        [DisplayName("UUID")]
        public string MemberUUId { get; set; } = string.Empty;
        [MaxLength(45)]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [MaxLength(45)]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get ; set; }
        [MaxLength(50)] 
        public string Email { get; set; }
        [MaxLength(13)]
        [DisplayName("Phone")]
        public string PhoneNumber { get; set; }
        public bool  Gender { get; set; }
        [MaxLength(50)] 
        public string Address { get; set; }
        [DisplayName("Joined Date")]
        [DataType(DataType.Date)]
        public DateTime JoinedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
        [DisplayName("Avatar")]
        public string? ImageUrl { get; set; }
        [ForeignKey("StudioID")]
       // [ValidateNever]
        public int StudioID { get; set; }
    }
}
