using Expense_Tracker.Models.Database;
using Expense_Tracker.Models;
using Expense_Tracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<BudgetController> _logger;
        public BudgetController(MongoDbContext mongoDbContext, ILogger<BudgetController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }



       
        public async Task<IActionResult> BudgetIndex()
        {
            var budgets = await _mongoDbContext.Budgets.Find(_ => true).ToListAsync();

            // Retrieve the categories as well for display
            //var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            var categories = await _mongoDbContext.Categories
    .Find(c => c.Type == "Expense")  // Filter categories where Type is 'Expense'
    .ToListAsync();

            var viewModel = budgets.Select(budget => new BudgetViewModel
            {
                BudgetId = budget.BudgetId,
                Amount = budget.Amount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                AlertThreshold = budget.AlertThreshold,
                CategoryName = categories.FirstOrDefault(c => c.CategoryId == budget.CategoryId)?.Title ?? "Unknown"
            }).ToList();

            ViewBag.Categories = categories;

            return View("~/Views/Budget/BudgetIndex.cshtml", viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBudget(Budget budget)
        {

            budget.BudgetId = Guid.NewGuid();
            budget.CreatedDate = DateTime.Now;
                // Save the budget to the database
            await _mongoDbContext.Budgets.InsertOneAsync(budget);
          
            return RedirectToAction("BudgetIndex");
        }


        public async Task<IActionResult> EditBudget(Guid id)
        {
            var budget = await _mongoDbContext.Budgets.Find(e => e.BudgetId == id).FirstOrDefaultAsync();
            if (budget == null)
            {
                return NotFound("Budget not found.");
            }

            // Fetch category details
            var category = await _mongoDbContext.Categories.Find(c => c.CategoryId == budget.CategoryId).FirstOrDefaultAsync();
            string categoryName = category?.Title ?? "Unknown Category";

            // Map to a view model
            var model = new BudgetViewModel
            {
                BudgetId = budget.BudgetId,
                Amount = budget.Amount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                AlertThreshold = budget.AlertThreshold,
                CategoryId = budget.CategoryId,
                CategoryName = categoryName
            };

            // Populate the dropdown
            ViewBag.Categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            return PartialView("~/Views/Budget/_EditBudgetPartial.cshtml", model);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBudget(BudgetViewModel model)
        {
           

            var budget = await _mongoDbContext.Budgets.Find(b => b.BudgetId == model.BudgetId).FirstOrDefaultAsync();
            if (budget == null)
            {
                return NotFound();
            }

            // Update the budget properties
            budget.Amount = model.Amount;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;
            budget.AlertThreshold = model.AlertThreshold;
       
            await _mongoDbContext.Budgets.ReplaceOneAsync(b => b.BudgetId == model.BudgetId, budget);

            return RedirectToAction("BudgetIndex");
        }

      
      
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var filter = Builders<Budget>.Filter.Eq(c => c.BudgetId, id);
            var result = await _mongoDbContext.Budgets.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }



        public async Task<IActionResult> BudgetDashboard()
        {
            // Retrieve all budgets for the current year using MongoDB's filter
            var currentYear = DateTime.Now.Year;
            var budgetFilter = Builders<Budget>.Filter.Gte(b => b.StartDate, new DateTime(currentYear, 1, 1)) &
                               Builders<Budget>.Filter.Lte(b => b.StartDate, new DateTime(currentYear, 12, 31));

            var budgets = await _mongoDbContext.Budgets.Find(budgetFilter).ToListAsync();

            // Calculate the total budgeted amount for the year
            var totalBudgetedAmount = budgets.Sum(b => b.Amount);

            // Retrieve all expenses for the current year using MongoDB's filter
            var expenseFilter = Builders<Expense>.Filter.Gte(e => e.CreatedDate, new DateTime(currentYear, 1, 1)) &
                                Builders<Expense>.Filter.Lte(e => e.CreatedDate, new DateTime(currentYear, 12, 31));

            var expenses = await _mongoDbContext.Expenses.Find(expenseFilter).ToListAsync();

            // Calculate the total spent amount for the year
            var totalSpentAmount = expenses.Sum(e => e.Amount);

            // Calculate remaining budget for the year
            var remainingBudget = totalBudgetedAmount - totalSpentAmount;

            // Group budgets by month (StartDate)
            var groupedBudgets = budgets
                .GroupBy(b => b.StartDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalBudgetedAmount = g.Sum(b => b.Amount)
                })
                .ToList();

            // Group expenses by month (CreatedDate)
            var groupedExpenses = expenses
                .GroupBy(e => e.CreatedDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalSpentAmount = g.Sum(e => e.Amount)
                })
                .ToList();

            // Combine the budget and expense breakdown by month
            var budgetSpentBreakdown = new List<dynamic>();
            for (int month = 1; month <= 12; month++)
            {
                var budget = groupedBudgets.FirstOrDefault(b => b.Month == month);
                var expense = groupedExpenses.FirstOrDefault(e => e.Month == month);

                budgetSpentBreakdown.Add(new
                {
                    Month = month,
                    MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalBudgetedAmount = budget?.TotalBudgetedAmount ?? 0,
                    TotalSpentAmount = expense?.TotalSpentAmount ?? 0,
                    RemainingBudget = (budget?.TotalBudgetedAmount ?? 0) - (expense?.TotalSpentAmount ?? 0)
                });
            }

            // Retrieve all categories
            var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            // Group expenses by CategoryId and map to Category titles
            var categoryBreakdown = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new
                {
                    CategoryTitle = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.Title ?? "Unknown",
                    TotalSpent = g.Sum(e => e.Amount)
                })
                .ToList();

            // Pass total values to the view via ViewBag
            ViewBag.TotalBudgetedAmount = totalBudgetedAmount;
            ViewBag.TotalSpentAmount = totalSpentAmount;
            ViewBag.RemainingBudget = remainingBudget;

            // Pass data to the view
            ViewBag.BudgetSpentBreakdown = budgetSpentBreakdown; // Breakdown of budget and expenses by month
            ViewBag.CategoryBreakdown = categoryBreakdown; // Category breakdown of expenses

            return View("~/Views/Budget/BudgetDashboard.cshtml");
        }






        //[HttpGet]
        //public async Task<IActionResult> GetFilteredData(int? month, string category)
        //{
        //    // Retrieve all budgets from the database
        //    var budgets = await _mongoDbContext.Budgets.Find(_ => true).ToListAsync();

        //    // Retrieve all expenses from the database
        //    var expenses = await _mongoDbContext.Expenses.Find(e => true).ToListAsync();

        //    // Apply the month filter to expenses
        //    if (month.HasValue)
        //    {
        //        expenses = expenses.Where(e => e.CreatedDate.Month == month.Value).ToList();
        //    }

        //    // Retrieve all categories
        //    var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

        //    // Filter expenses by category if a category is specified
        //    if (!string.IsNullOrEmpty(category))
        //    {
        //        var selectedCategory = categories.FirstOrDefault(c => c.Title == category);
        //        if (selectedCategory != null)
        //        {
        //            expenses = expenses.Where(e => e.CategoryId == selectedCategory.CategoryId).ToList();
        //        }
        //    }

        //    // Calculate the total budgeted amount (remains the same regardless of filters)
        //    var totalBudgetedAmount = budgets.Sum(b => b.Amount);

        //    // Calculate the total spent amount after applying filters
        //    var totalSpentAmount = expenses.Sum(e => e.Amount);

        //    // Group filtered expenses by CategoryId and map to Category titles
        //    var categoryBreakdown = expenses
        //        .GroupBy(e => e.CategoryId)
        //        .Select(g => new
        //        {
        //            CategoryTitle = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.Title ?? "Unknown",
        //            TotalSpent = g.Sum(e => e.Amount)
        //        })
        //        .ToList();

        //    // Calculate the remaining budget
        //    var remainingBudget = totalBudgetedAmount - totalSpentAmount;

        //    // Prepare data for the bar chart
        //    var categoryTitles = categoryBreakdown.Select(c => c.CategoryTitle).ToList();
        //    var spentAmounts = categoryBreakdown.Select(c => c.TotalSpent).ToList();

        //    // Return the filtered data as JSON
        //    return Json(new
        //    {
        //        TotalBudgetedAmount = totalBudgetedAmount,
        //        TotalSpentAmount = totalSpentAmount,
        //        RemainingBudget = remainingBudget,
        //        CategoryBreakdown = categoryBreakdown,
        //        CategoryTitles = categoryTitles,
        //        SpentAmounts = spentAmounts
        //    });
        //}


        public async Task<IActionResult> GetFilteredData(int? month, string category)
        {
            // Create a filter definition for the budget
            var filterBuilder = Builders<Budget>.Filter;
            var filter = filterBuilder.Empty; // Start with no filter (i.e., get everything)

            // Apply the filter for the selected month (based on StartDate)
            if (month.HasValue && month > 0)
            {
                filter &= filterBuilder.Where(b => b.StartDate.Month == month.Value);
            }

            // Apply the filter for the selected category
            if (!string.IsNullOrEmpty(category))
            {
                // Find the CategoryId corresponding to the selected category
                var categoryId = (await _mongoDbContext.Categories
                    .Find(c => c.Title == category)
                    .FirstOrDefaultAsync())?.CategoryId;

                if (categoryId != null)
                {
                    // Apply the category filter to the budget
                    filter &= filterBuilder.Where(b => b.CategoryId == categoryId);
                }
            }

            // Retrieve the filtered budgets
            var filteredBudgets = await _mongoDbContext.Budgets
                                                       .Find(filter)
                                                       .ToListAsync();

            // Get the CategoryIds of the filtered budgets
            var budgetCategoryIds = filteredBudgets.Select(b => b.CategoryId.ToString()).Distinct().ToList();

            // Create a filter for expenses, filtering by CategoryId and matching the selected month
            var expenseFilter = Builders<Expense>.Filter.In(e => e.CategoryId.ToString(), budgetCategoryIds);

            if (month.HasValue && month > 0)
            {
                expenseFilter &= Builders<Expense>.Filter.Where(e => e.CreatedDate.Month == month.Value);
            }

            // Retrieve filtered expenses based on the constructed filter
            var filteredExpenses = await _mongoDbContext.Expenses
                                                         .Find(expenseFilter)
                                                         .ToListAsync();

            // Calculate total spent amount
            var totalSpentAmount = filteredExpenses.Sum(e => e.Amount);

            // Retrieve categories outside the GroupBy, so we don't use async calls inside LINQ
            var categoryTitles = await _mongoDbContext.Categories
                                                       .Find(c => budgetCategoryIds.Contains(c.CategoryId.ToString()))
                                                       .ToListAsync();

            // Group expenses by CategoryId and calculate total spent per category
            var categoryBreakdown = filteredExpenses
                .GroupBy(e => e.CategoryId)
                .Select(g =>
                {
                    // Find category title by matching the CategoryId with the pre-fetched categories
                    var category = categoryTitles.FirstOrDefault(c => c.CategoryId == g.Key);
                    var categoryTitle = category?.Title ?? "Unknown";

                    return new
                    {
                        CategoryTitle = categoryTitle,
                        TotalSpent = g.Sum(e => e.Amount)
                    };
                })
                .ToList();

            // Calculate the total budgeted amount for the filtered budgets
            var totalBudgetedAmount = filteredBudgets.Sum(b => b.Amount);

            // Calculate the remaining budget
            var remainingBudget = totalBudgetedAmount - totalSpentAmount;

            // Prepare the data to pass to the view
            var data = new
            {
                TotalBudgetedAmount = totalBudgetedAmount,
                TotalSpentAmount = totalSpentAmount,
                RemainingBudget = remainingBudget,
                CategoryBreakdown = categoryBreakdown,
                CategoryTitles = categoryBreakdown.Select(c => c.CategoryTitle).ToList(),
                SpentAmounts = categoryBreakdown.Select(c => c.TotalSpent).ToList()
            };

            return Json(data);
        }





    }

}
