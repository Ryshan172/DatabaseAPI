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

    public class UserWithContactModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleID { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CreateUserModel
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CreateUniversityUserModel
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentID { get; set; }
        public int UniversityID { get; set; }
    }


}