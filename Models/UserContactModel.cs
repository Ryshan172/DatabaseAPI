using System;

namespace DatabaseApi.Models
{

    public record UserContactModel
    {
        public int ContactID{ get; init; }
        public int UserID{ get; init; }
        public string Email{ get; init;}
        public string PhoneNumber{ get; init; }
    }
}