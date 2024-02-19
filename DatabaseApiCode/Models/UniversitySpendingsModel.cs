public record UniversitySpendingsModel
{
    public int AllocationYear { get; init; }
    public int UniversityID { get; init; }
    public decimal TotalAmount { get; set; }
    public decimal AmountRemaining { get; set; }
    public List<StudentInfoModel> FundedStudents { get; set; }

}