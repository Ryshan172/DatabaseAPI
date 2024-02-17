using System;

namespace DatabaseApi.Models
{
    public record StudentAllocationModel 
    {
        // public int AllocationID { get; set; }
        // Don't need AllocationID because generated by table 
        public decimal Amount { get; init; }
        public int AllocationYear { get; init; }
        public int StudentID {get; init;}
        public int ApplicationStatusID {get; init;}
        
    }
}