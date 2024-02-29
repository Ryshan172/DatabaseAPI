#pragma warning disable CS1591


namespace DatabaseApiCode.Models
{
    public record UniversityApplicationModel
    {
        public int ApplicationID { get; init; }

        public int ApplicationStatusID { get; init; } = 1;

        public decimal AmountRequested { get; init; }

        public int UniversityID {get; init; }

        public int ApplicationYear {get; init; } = DateTime.Now.Year;

        // Actually an int because 1 or 0 even though BIT on Database
        public bool IsLocked {get; init; } = true;
        

    }


    public record UniversityApplicationModelWithName
    {
        public int ApplicationID { get; init; }

        public int ApplicationStatusID { get; init; }

        public decimal AmountRequested { get; init; }

        public int UniversityID {get; init; }

        public int ApplicationYear {get; init; }

        // Actually an int because 1 or 0 even though BIT on Database
        public bool IsLocked {get; init; }

        public string UniversityName {get; init; }
        

    }

    
}
