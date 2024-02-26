#pragma warning disable CS1591

using System.ComponentModel.DataAnnotations;

namespace DatabaseApiCode.Models
{  
    public record BursaryAllocationModel

    {
        // public int ApplicationID{get; set;}
        public int UniversityID { get; init; }

        [Required(ErrorMessage = "AmountAllocated is required.")]
        public decimal AmountAlloc { get; init; }

        [Required(ErrorMessage = "AllocatedDate is required.")]
        public int AllocationYear { get; init; }

        [Required(ErrorMessage = "UniversityApplication is required.")]
        public int UniversityApplicationID { get; init; }

    }
}
