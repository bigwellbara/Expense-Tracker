using Expense_Tracker.Models;

namespace Expense_Tracker.ViewModels
{
    public class BudgetViewModel
    {
        public Guid BudgetId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double AlertThreshold { get; set; }
        public string CategoryName { get; set; } // Add this line
    }

}
