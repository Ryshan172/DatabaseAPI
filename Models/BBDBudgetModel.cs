namespace DatabaseApiCode.Models
{
    public class BBDBudgetModel
    {
        public decimal Budget { get; set; }

        public decimal AmountAllocated { get; set; }

        public int BudgetYear { get; set; }

    }

    public class BursaryHistoryModel
    {
        public decimal AmountAllocated { get; init; }

        public int AllocationYear { get; init; }

        public string UniName { get; init; }
        public int AllocationID { get; init; }

    }

    public class AllocationBudgetModel{

        public string UniName { get; init; }
        public decimal AmountAllocated { get; init; }
    }
}