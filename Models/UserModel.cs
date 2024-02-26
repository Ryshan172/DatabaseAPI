#pragma warning disable CS1591
#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace DatabaseApiCode.Models
{
    public class UserModel
    {

        [Key]
        public int UserID{get;set;}

        [Required]
        [StringLength(255)]
        public string FirstName{get;set;}

        [Required]
        [StringLength(255)]
        public string LastName{get;set;}

        [Required]
        public int RoleID { get; set; }
    }
}