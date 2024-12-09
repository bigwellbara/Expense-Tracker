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


    }

}
