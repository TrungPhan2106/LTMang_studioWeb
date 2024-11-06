using MemberManagement.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace MemberManagement.Data
{
    public class Studio
    {
        [Key]
        [DisplayName("ID")]
        public int StudioID { get; set; }
        [MaxLength(45)]
        [DisplayName("Studio Name")]
        public string StudioName { get; set; }
        [MaxLength(100)]
        [DisplayName("Address")]
        public string StudioAddress { get; set; }
        [MaxLength(45)]
        [DisplayName("Contact")]
        public string StudioPhone { get; set; }
        public string? StudioPic { get; set; }
    }
}
