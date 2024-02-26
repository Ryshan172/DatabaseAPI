#pragma warning disable CS1591

namespace DatabaseApiCode.Models
{
    public record StudentAllocationModel 
    {
        public int AllocationID { get; set; }
        public decimal Amount { get; init; }

        public int AllocationYear { get; init; }

        public string StudentIDNum {get; init;}

        public int StudentMarks {get; init;}
        public int CourseYear {get; init;}
        public int ApplicationStatusID {get; init;}
        
    }


    public class CreateStudentApplicationModel
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentIDNum { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GenderID { get; set; }
        public int EthnicityID { get; set; }
        public int DepartmentID { get; set; }
        public int UniversityID { get; set; }
        public int Amount { get; set; }
        public int StudentMarks { get; set; }
        public int CourseYear { get; set; }
    }

}