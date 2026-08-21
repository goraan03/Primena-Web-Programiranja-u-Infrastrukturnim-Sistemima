namespace TravelPlanner.Contracts.Models
{
    public class BudgetSummaryDto
    {
        public int TravelId { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal RemainingBudget { get; set; }
        public List<CategorySummaryDto> ByCategory { get; set; } = new();
    }

    public class CategorySummaryDto
    {
        public string Category { get; set; }
        public decimal Total { get; set; }
    }
}