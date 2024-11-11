using Expense_Tracker.Models.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Expense_Tracker.Controllers
{
    public class ReportsController :Controller
    {

        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<ReportsController> _logger;
        public ReportsController(MongoDbContext mongoDbContext, ILogger<ReportsController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }



        //public async Task<IActionResult> IncomeExpenseDashboard()
        //{
        //    // Fetch incomes and expenses from MongoDB
        //    var incomes = await _mongoDbContext.Incomes.Find(_ => true).ToListAsync();
        //    var expenses = await _mongoDbContext.Expenses.Find(_ => true).ToListAsync();

        //    // Calculate total income and total expense
        //    var totalIncome = incomes.Sum(i => i.Amount);
        //    var totalExpense = expenses.Sum(e => e.Amount);

        //    // Calculate net balance
        //    var netBalance = totalIncome - totalExpense;

        //    // Current month data for income and expense
        //    var currentMonth = DateTime.Now.Month;
        //    var monthlyIncome = incomes
        //        .Where(i => i.IncomeDate.Month == currentMonth)
        //        .Sum(i => i.Amount);

        //    var monthlyExpense = expenses
        //        .Where(e => e.ExpenseDate.Month == currentMonth)
        //        .Sum(e => e.Amount);

        //    // Monthly data for trend chart
        //    var incomeByMonth = incomes
        //        .GroupBy(i => new { i.IncomeDate.Year, i.IncomeDate.Month })
        //        .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(i => i.Amount) })
        //        .OrderBy(g => g.Month)
        //        .ToList();

        //    var expenseByMonth = expenses
        //        .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
        //        .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(e => e.Amount) })
        //        .OrderBy(g => g.Month)
        //        .ToList();

        //    // Category breakdown for income and expenses
        //    var incomeByCategory = incomes
        //        .GroupBy(i => i.CategoryId)
        //        .Select(g => new { CategoryTitle = g.Key, Total = g.Sum(i => i.Amount) })
        //        .ToList();

        //    var expenseByCategory = expenses
        //        .GroupBy(e => e.CategoryId)
        //        .Select(g => new { CategoryTitle = g.Key, Total = g.Sum(e => e.Amount) })
        //        .ToList();

        //    // Prepare data for JavaScript
        //    ViewBag.TotalIncome = totalIncome;
        //    ViewBag.TotalExpense = totalExpense;
        //    ViewBag.NetBalance = netBalance;
        //    ViewBag.MonthlyIncome = monthlyIncome;
        //    ViewBag.MonthlyExpenses = monthlyExpense;

        //    ViewBag.IncomeByMonthData = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByMonth.Select(m => m.Total));
        //    ViewBag.ExpenseByMonthData = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByMonth.Select(m => m.Total));
        //    ViewBag.MonthLabels = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByMonth.Select(m => m.Month));

        //    ViewBag.IncomeCategoryTotals = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByCategory.Select(c => c.Total));
        //    ViewBag.IncomeCategoryLabels = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByCategory.Select(c => c.CategoryTitle));

        //    ViewBag.ExpenseCategoryTotals = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByCategory.Select(c => c.Total));
        //    ViewBag.ExpenseCategoryLabels = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByCategory.Select(c => c.CategoryTitle));

        //    return View("~/Views/IncomeExpenses/IncomeExpenseDashboard.cshtml");
        //}

        public async Task<IActionResult> IncomeExpenseDashboard()
        {
            // Fetch incomes and expenses from MongoDB
            var incomes = await _mongoDbContext.Incomes.Find(_ => true).ToListAsync();
            var expenses = await _mongoDbContext.Expenses.Find(_ => true).ToListAsync();

            // Fetch categories from the database
            var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            // Calculate total income and total expense
            var totalIncome = incomes.Sum(i => i.Amount);
            var totalExpense = expenses.Sum(e => e.Amount);

            // Calculate net balance
            var netBalance = totalIncome - totalExpense;

            // Current month data for income and expense
            var currentMonth = DateTime.Now.Month;
            var monthlyIncome = incomes
                .Where(i => i.IncomeDate.Month == currentMonth)
                .Sum(i => i.Amount);

            var monthlyExpense = expenses
                .Where(e => e.ExpenseDate.Month == currentMonth)
                .Sum(e => e.Amount);

            // Monthly data for trend chart
            var incomeByMonth = incomes
                .GroupBy(i => new { i.IncomeDate.Year, i.IncomeDate.Month })
                .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(i => i.Amount) })
                .OrderBy(g => g.Month)
                .ToList();

            var expenseByMonth = expenses
                .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(e => e.Amount) })
                .OrderBy(g => g.Month)
                .ToList();

            // Category breakdown for income and expenses
            var incomeByCategory = incomes
                .GroupBy(i => i.CategoryId)
                .Select(g => new { CategoryTitle = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.Title ?? "Unknown", Total = g.Sum(i => i.Amount) })
                .ToList();

            var expenseByCategory = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new { CategoryTitle = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.Title ?? "Unknown", Total = g.Sum(e => e.Amount) })
                .ToList();

            // Prepare data for JavaScript
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.NetBalance = netBalance;
            ViewBag.MonthlyIncome = monthlyIncome;
            ViewBag.MonthlyExpenses = monthlyExpense;

            ViewBag.IncomeByMonthData = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByMonth.Select(m => m.Total));
            ViewBag.ExpenseByMonthData = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByMonth.Select(m => m.Total));
            ViewBag.MonthLabels = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByMonth.Select(m => m.Month));

            ViewBag.IncomeCategoryTotals = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByCategory.Select(c => c.Total));
            ViewBag.IncomeCategoryLabels = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByCategory.Select(c => c.CategoryTitle));

            ViewBag.ExpenseCategoryTotals = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByCategory.Select(c => c.Total));
            ViewBag.ExpenseCategoryLabels = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByCategory.Select(c => c.CategoryTitle));

            return View("~/Views/IncomeExpenses/IncomeExpenseDashboard.cshtml");
        }


        public async Task<IActionResult> IncomeExpenseDashboardFilter(int? year, int? month)
        {
            // Fetch incomes and expenses from MongoDB
            var incomes = await _mongoDbContext.Incomes.Find(_ => true).ToListAsync();
            var expenses = await _mongoDbContext.Expenses.Find(_ => true).ToListAsync();


            // Fetch categories from the database
            var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            // Filter by year if provided
            if (year.HasValue)
            {
                incomes = incomes.Where(i => i.IncomeDate.Year == year.Value).ToList();
                expenses = expenses.Where(e => e.ExpenseDate.Year == year.Value).ToList();
            }

            // Filter by month if provided
            if (month.HasValue)
            {
                incomes = incomes.Where(i => i.IncomeDate.Month == month.Value).ToList();
                expenses = expenses.Where(e => e.ExpenseDate.Month == month.Value).ToList();
            }

            // Calculate total income and total expense
            var totalIncome = incomes.Sum(i => i.Amount);
            var totalExpense = expenses.Sum(e => e.Amount);

            // Calculate net balance
            var netBalance = totalIncome - totalExpense;

            // Current month data for income and expense
            var currentMonth = DateTime.Now.Month;
            var monthlyIncome = incomes
                .Where(i => i.IncomeDate.Month == (month ?? currentMonth))
                .Sum(i => i.Amount);

            var monthlyExpense = expenses
                .Where(e => e.ExpenseDate.Month == (month ?? currentMonth))
                .Sum(e => e.Amount);

            // Monthly data for trend chart
            var incomeByMonth = incomes
                .GroupBy(i => new { i.IncomeDate.Year, i.IncomeDate.Month })
                .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(i => i.Amount) })
                .OrderBy(g => g.Month)
                .ToList();

            var expenseByMonth = expenses
                .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(e => e.Amount) })
                .OrderBy(g => g.Month)
                .ToList();

            //// Category breakdown for income and expenses
            //var incomeByCategory = incomes
            //    .GroupBy(i => i.CategoryId)
            //    .Select(g => new { CategoryTitle = g.Key, Total = g.Sum(i => i.Amount) })
            //    .ToList();

            //var expenseByCategory = expenses
            //    .GroupBy(e => e.CategoryId)
            //    .Select(g => new { CategoryTitle = g.Key, Total = g.Sum(e => e.Amount) })
            //    .ToList();

            // Category breakdown for income and expenses
            var incomeByCategory = incomes
                .GroupBy(i => i.CategoryId)
                .Select(g => new { CategoryTitle = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.Title ?? "Unknown", Total = g.Sum(i => i.Amount) })
                .ToList();

            var expenseByCategory = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new { CategoryTitle = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.Title ?? "Unknown", Total = g.Sum(e => e.Amount) })
                .ToList();

            // Prepare data for JavaScript
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.NetBalance = netBalance;
            ViewBag.MonthlyIncome = monthlyIncome;
            ViewBag.MonthlyExpenses = monthlyExpense;

            ViewBag.IncomeByMonthData = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByMonth.Select(m => m.Total));
            ViewBag.ExpenseByMonthData = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByMonth.Select(m => m.Total));
            ViewBag.MonthLabels = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByMonth.Select(m => m.Month));

            ViewBag.IncomeCategoryTotals = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByCategory.Select(c => c.Total));
            ViewBag.IncomeCategoryLabels = Newtonsoft.Json.JsonConvert.SerializeObject(incomeByCategory.Select(c => c.CategoryTitle));

            ViewBag.ExpenseCategoryTotals = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByCategory.Select(c => c.Total));
            ViewBag.ExpenseCategoryLabels = Newtonsoft.Json.JsonConvert.SerializeObject(expenseByCategory.Select(c => c.CategoryTitle));

            return View("~/Views/IncomeExpenses/IncomeExpenseDashboard.cshtml");
        }







    }
}
