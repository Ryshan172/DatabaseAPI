#pragma warning disable CS1591
#pragma warning disable CS8618


public record UniversitySpendingsModel
{
    public int AllocationYear { get; init; }
    public int UniversityID { get; init; }
    public decimal TotalAmount { get; set; }
    public decimal AmountRemaining { get; set; }
    public List<StudentInfoModel> FundedStudents { get; set; }

}