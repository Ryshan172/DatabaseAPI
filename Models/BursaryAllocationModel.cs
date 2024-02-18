using System.ComponentModel.DataAnnotations;


namespace DatabaseApi.Models
{  
    public record BursaryAllocationModel

    {
        // public int ApplicationID{get; set;}
        public int UniversityID{ get; init; }

        [Required(ErrorMessage = "AmountAllocated is required.")]
        public decimal AmountAllocated{ get; init; }

        [Required(ErrorMessage = "AllocatedDate is required.")]
        public int AllocatedYear{ get; init; }

    }
}
