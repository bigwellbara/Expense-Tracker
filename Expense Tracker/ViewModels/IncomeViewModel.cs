namespace Expense_Tracker.ViewModels
{
    public class IncomeViewModel
    {
        public IEnumerable<Models.Income> Income { get; set; }
        public IEnumerable<Models.Category> Categories { get; set; }
    }
}
