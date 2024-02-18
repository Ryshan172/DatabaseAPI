namespace DatabaseApi.Models
{
    public class StudentModel : UserModel
    {
        public int StudentID { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int GenderID { get; set; }

        public int EthnicityID { get; set; }

        public int DepartmentID { get; set; }

        public int UniversityID { get; set; }

    }
}
