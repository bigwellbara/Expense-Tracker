using Expense_Tracker.Models.Database;
using Expense_Tracker.Models;
using Expense_Tracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Expense_Tracker.Controllers
{
    public class BudgetController : Controller
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<BudgetController> _logger;
        public BudgetController(MongoDbContext mongoDbContext, ILogger<BudgetController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }


        //public async Task<IActionResult> Index()
        //{
        //    // Retrieve all budgets for the current user (assume UserId is available)
        //    //var userId = /* retrieve current user's ID */;
        //    var budgets = await _mongoDbContext.Budgets
        //        .Find(b => b.UserId == userId)
        //        .ToListAsync();

        //    // Optionally load categories to display category names instead of IDs
        //    var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();
        //    var viewModel = budgets.Select(b => new BudgetViewModel
        //    {
        //        Amount = b.Amount,
        //        StartDate = b.StartDate,
        //        EndDate = b.EndDate,
        //        AlertThreshold = b.AlertThreshold,
        //        CategoryId = b.CategoryId,
        //        Categories = categories // Passing categories to map CategoryId to names
        //    }).ToList();

        //    return View(viewModel);
        //}

        public async Task<IActionResult> BudgetIndex()
        {
            var budgets = await _mongoDbContext.Budgets.Find(_ => true).ToListAsync();

            // Retrieve the categories as well for display
            var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

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

        //public async Task<IActionResult> BudgetIndex()
        //{
        //    var budgets = await _mongoDbContext.Budgets.Find(_ => true).ToListAsync();

        //    // Retrieve the categories as well for display
        //    var categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

        //    //var viewModel = budgets.Select(budget => new BudgetViewModel
        //    //{
        //    //    BudgetId = budget.BudgetId,
        //    //    Amount = budget.Amount,
        //    //    StartDate = budget.StartDate,
        //    //    EndDate = budget.EndDate,
        //    //    AlertThreshold = budget.AlertThreshold,
        //    //    CategoryName = categories.FirstOrDefault(c => c.CategoryId == budget.CategoryId)?.Title ?? "Unknown"
        //    //}).ToList();

        //    ViewBag.Categories = categories;

        //    return View("~/Views/Budget/BudgetIndex.cshtml", budgets);
        //}

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateBudget(Budget updatedBudget)
        //{
        //    try
        //    {
        //        // Check if the updatedIncome object or its IncomeId is null/empty
        //        if (updatedBudget == null || updatedBudget.BudgetId == Guid.Empty)
        //        {
        //            _logger.LogError("Budget or BudgetId is null/empty.");
        //            TempData["ErrorMessage"] = "Invalid Budget data.";
        //            return RedirectToAction("BudgetIndex"); // Adjust according to your routing
        //        }

        //        // Define the filter to find the specific income record to update
        //        var filter = Builders<Budget>.Filter.Eq(i => i.BudgetId, updatedBudget.BudgetId);

        //        // Set the fields to update
        //        var update = Builders<Budget>.Update
        //            .Set(i => i.AlertThreshold, updatedBudget.AlertThreshold)
        //            .Set(i => i.Amount, updatedBudget.Amount)
        //            .Set(i => i.StartDate, updatedBudget.StartDate)
        //            .Set(i => i.EndDate, updatedBudget.EndDate)
        //            .Set(i => i.CategoryId, updatedBudget.CategoryId);

        //        // Perform the update operation
        //        var result = await _mongoDbContext.Budgets.UpdateOneAsync(filter, update);

        //        // Check if any document was modified
        //        if (result.ModifiedCount == 0)
        //        {
        //            TempData["ErrorMessage"] = "Budget not found or could not be updated.";
        //        }
        //        else
        //        {
        //            TempData["SuccessMessage"] = "Budget updated successfully.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and set an error message
        //        _logger.LogError(ex, "An error occurred while updating the budget.");
        //        TempData["ErrorMessage"] = "An error occurred while processing your request.";
        //    }

        //    // Redirect to the IncomeIndex view to show messages
        //    return RedirectToAction("BudgetIndex"); // Adjust according to your routing
        //}

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




    }

}
