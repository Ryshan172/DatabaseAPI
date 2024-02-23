#pragma warning disable CS1591

namespace DatabaseApiCode.Models
{
    public class StudentModel : UserModel
    {
        public string StudentIDNum { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int GenderID { get; set; }

        public int EthnicityID { get; set; }

        public int DepartmentID { get; set; }

        public int UniversityID { get; set; }

    }
}
