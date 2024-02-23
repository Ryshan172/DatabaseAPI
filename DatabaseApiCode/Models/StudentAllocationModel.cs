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
        public int ApplicationStatusID {get; init;}
        
    }
}