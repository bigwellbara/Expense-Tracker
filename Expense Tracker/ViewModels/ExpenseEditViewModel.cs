using Expense_Tracker.Models;

namespace Expense_Tracker.ViewModels
{
    public class ExpenseEditViewModel
    {
        public Expense Expense { get; set; }
        public List<Category> Categories { get; set; }
    }
}
