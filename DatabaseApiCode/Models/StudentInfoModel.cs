#pragma warning disable CS1591
#pragma warning disable CS8618
public record StudentInfoModel
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public decimal AllocationAmount { get; init; }

    public string StudentIDNum { get; init; }

}
