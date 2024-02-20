#pragma warning disable CS1591
#pragma warning disable CS8618

namespace DatabaseApiCode.Models
{
    public record UserContactModel
    {
        public int ContactID{ get; init; }

        public int UserID{ get; init; }

        public string Email{ get; init;}

        public string PhoneNumber{ get; init; }

    }
}