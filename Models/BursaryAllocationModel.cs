using System.ComponentModel.DataAnnotations;


namespace DatabaseApi.Models{
    
    public class BursaryAllocationModel{

        // public int ApplicationID{get; set;}
        public int UniversityID{get;set;}

        [Required(ErrorMessage = "AmountAllocated is required.")]
        public decimal AmountAllocated{get;set;}

        [Required(ErrorMessage = "AllocatedDate is required.")]
        public DateTime AllocatedYear{get;set;}
    }
}
