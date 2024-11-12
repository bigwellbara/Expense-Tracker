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

        public BudgetController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBudget(Budget budget)
        {
            if (ModelState.IsValid)
            {
                // Save the budget to the database
                await _mongoDbContext.Budgets.InsertOneAsync(budget);
                return RedirectToAction("Index");
            }

            return RedirectToAction("BudgetIndex");
        }



    }

}
