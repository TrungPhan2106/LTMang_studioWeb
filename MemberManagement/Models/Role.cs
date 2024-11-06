using Microsoft.AspNetCore.Identity;
using StudioManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace StudioManagement.Models
{
    public class Role : IdentityRole<int>
    {
        [Key]
        public new int Id { get; set; }
        public string rolename { get; set; }
        public ICollection<User> User { get; set; }
    }
}