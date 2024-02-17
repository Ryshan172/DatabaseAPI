using System;

namespace DatabaseApi.Models
{
    public record UniApplicationModel
    {
        public int ApplicationStatusID { get; init; }
        public int AmountRequested { get; init; }
        public int UniversityID {get; init; }
    }
}
