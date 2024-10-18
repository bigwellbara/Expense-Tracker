namespace Expense_Tracker.ViewModels
{
    public class ExpensesViewModel
    {
        public IEnumerable<Models.Expense> Expenses { get; set; }
        public IEnumerable<Models.Category> Categories { get; set; }
    }
}
