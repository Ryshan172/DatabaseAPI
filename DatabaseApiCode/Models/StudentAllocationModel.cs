namespace DatabaseApiCode.Models
{
    public record StudentAllocationModel 
    {
        public int AllocationID { get; set; }
        public decimal Amount { get; init; }

        public int AllocationYear { get; init; }

        public int StudentID {get; init;}
        public int ApplicationStatusID {get; init;}
        
    }
}