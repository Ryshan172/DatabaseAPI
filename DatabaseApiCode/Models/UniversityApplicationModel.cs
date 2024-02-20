#pragma warning disable CS1591


namespace DatabaseApiCode.Models
{
    public record UniversityApplicationModel
    {
        public int ApplicationID { get; init; }

        public int ApplicationStatusID { get; init; }

        public decimal AmountRequested { get; init; }

        public int UniversityID {get; init; }

    }
}
