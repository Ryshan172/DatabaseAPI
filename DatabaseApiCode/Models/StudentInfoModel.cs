public record StudentInfoModel
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public decimal AllocationAmount { get; init; }

    public decimal  AmountRemaining{ get; init; }
}
