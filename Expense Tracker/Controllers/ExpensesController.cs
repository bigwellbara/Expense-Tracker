using Expense_Tracker.Models;
using Expense_Tracker.Models.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using Expense_Tracker.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {

        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController( MongoDbContext mongoDbContext, ILogger<ExpensesController> logger)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
        }
        //public async Task<IActionResult> ExpensesIndex()
        //{
        //    var allExpenses = await _mongoDbContext.Expenses.Find(_ => true).ToListAsync();
        //    // Explicitly specify the path to the view
        //    return View("~/Views/IncomeExpenses/ExpensesIndex.cshtml", allExpenses);
        //}


        public async Task<IActionResult> ExpensesIndex()
        {
            var allExpenses = await _mongoDbContext.Expenses.Find(_ => true).ToListAsync();
            //var allCategories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();

            // Fetch only categories where Type is 'Expense'
            var expenseCategories = await _mongoDbContext.Categories
                .Find(category => category.Type == "Expense")
                .ToListAsync();

            var viewModel = new ExpensesViewModel
            {
                Expenses = allExpenses,
                Categories = expenseCategories
            };

            // Explicitly specify the path to the view
            return View("~/Views/IncomeExpenses/ExpensesIndex.cshtml", viewModel);
        }




        //public async Task<IActionResult> CreateExpense(decimal amount, string title, string description, DateTime expenseDate, Guid userId, Guid categoryId)
        //{
        //    try
        //    {
        //        // Create a new expense object
        //        var newExpense = new Expense
        //        {
        //            Amount = amount,
        //            Title = title,
        //            Description = description,
        //            ExpenseDate = expenseDate,
        //            UserId = userId,
        //            CategoryId = categoryId,
        //            CreatedDate = DateTime.Now
        //        };

        //        // Insert the new expense into MongoDB
        //        await _mongoDbContext.Expenses.InsertOneAsync(newExpense);

        //        // Return success message or redirect to an appropriate view
        //        return RedirectToAction("ExpensesIndex");  // Assuming the Index view lists all expenses
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and return an error view
        //        // Replace 'LogError' with your logging mechanism
        //        //LogError(ex.Message);
        //        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> CreateExpense(Expense expense)
        {
            try
            {
                // Ensure that the ExpenseId and CreatedDate are set properly
                expense.ExpenseId = Guid.NewGuid();
                expense.CreatedDate = DateTime.Now;

                // Insert the new expense into MongoDB
                await _mongoDbContext.Expenses.InsertOneAsync(expense);

                // Redirect to the index view or show success message
                return RedirectToAction("ExpensesIndex");
            }
            catch (Exception ex)
            {
          
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }



        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var expense = await _mongoDbContext.Expenses.Find(x => x.ExpenseId == id).FirstOrDefaultAsync();
        //    if (expense == null) return NotFound();

        //    var viewModel = new ExpenseEditViewModel
        //    {
        //        Expense = expense,
        //        Categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync()
        //    };

        //    return PartialView("~/Views/IncomeExpenses/_EditExpensePartial.cshtml", viewModel);

         
        //}


        public async Task<IActionResult> Edit(Guid id)
        {
            var expense = await _mongoDbContext.Expenses.Find(e => e.ExpenseId == id).FirstOrDefaultAsync();
            if (expense == null)
            {
                return NotFound();
            }

            // Load categories for the dropdown
            ViewBag.Categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();
            return PartialView("~/Views/IncomeExpenses/_EditExpensePartial.cshtml", expense);
        }




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateExpense(Guid id, ExpenseEditViewModel model)
        //{
        //    if (id != model.Expense.ExpenseId)
        //    {
        //        return Json(new { success = false, message = "Bad Request: ID mismatch." });
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        // Collect validation errors and return as JSON
        //        var errors = ModelState.Values.SelectMany(v => v.Errors)
        //                                      .Select(e => e.ErrorMessage)
        //                                      .ToList();

        //        return Json(new { success = false, errors = errors });
        //    }

        //    var filter = Builders<Expense>.Filter.Eq(e => e.ExpenseId, model.Expense.ExpenseId);
        //    var update = Builders<Expense>.Update
        //        .Set(e => e.Title, model.Expense.Title)
        //        .Set(e => e.Amount, model.Expense.Amount)
        //        .Set(e => e.Description, model.Expense.Description)
        //        .Set(e => e.ExpenseDate, model.Expense.ExpenseDate)
        //        .Set(e => e.CategoryId, model.Expense.CategoryId);

        //    var result = await _mongoDbContext.Expenses.UpdateOneAsync(filter, update);

        //    if (result.ModifiedCount == 0)
        //    {
        //        return Json(new { success = false, message = "Expense not found or could not be updated." });
        //    }

        //    return Json(new { success = true, message = "Expense updated successfully." });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateExpense(Expense updatedExpense)
        //{
        //    if (updatedExpense == null || updatedExpense.ExpenseId == Guid.Empty)
        //    {
        //        _logger.LogError("Expense or ExpenseId is null/empty.");
        //        return Json(new { success = false, message = "Invalid Expense data." });
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors)
        //                                      .Select(e => e.ErrorMessage)
        //                                      .ToList();
        //        return Json(new { success = false, errors });
        //    }

        //    var filter = Builders<Expense>.Filter.Eq(e => e.ExpenseId, updatedExpense.ExpenseId);
        //    var update = Builders<Expense>.Update
        //        .Set(e => e.Title, updatedExpense.Title)
        //        .Set(e => e.Amount, updatedExpense.Amount)
        //        .Set(e => e.Description, updatedExpense.Description)
        //        .Set(e => e.ExpenseDate, updatedExpense.ExpenseDate)
        //        .Set(e => e.CategoryId, updatedExpense.CategoryId);

        //    var result = await _mongoDbContext.Expenses.UpdateOneAsync(filter, update);

        //    if (result.ModifiedCount == 0)
        //    {
        //        return Json(new { success = false, message = "Expense not found or could not be updated." });
        //    }

        //    return Json(new { success = true });
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateExpense(Expense updatedExpense)
        //{
        //    try
        //    {
        //        if (updatedExpense == null || updatedExpense.ExpenseId == Guid.Empty)
        //        {
        //            _logger.LogError("Expense or ExpenseId is null/empty.");
        //            TempData["ErrorMessage"] = "Invalid Expense data."; // Error message for invalid data
        //            return Json(new { success = false, message = "Invalid Expense data." });
        //        }

        //        var filter = Builders<Expense>.Filter.Eq(e => e.ExpenseId, updatedExpense.ExpenseId);
        //        var update = Builders<Expense>.Update
        //            .Set(e => e.Title, updatedExpense.Title)
        //            .Set(e => e.Amount, updatedExpense.Amount)
        //            .Set(e => e.Description, updatedExpense.Description)
        //            .Set(e => e.ExpenseDate, updatedExpense.ExpenseDate)
        //            .Set(e => e.CategoryId, updatedExpense.CategoryId);

        //        var result = await _mongoDbContext.Expenses.UpdateOneAsync(filter, update);

        //        if (result.ModifiedCount == 0)
        //        {
        //            TempData["ErrorMessage"] = "Expense not found or could not be updated."; // Error message for not found
        //            return Json(new { success = false, message = "Expense not found or could not be updated." });
        //        }

        //        TempData["SuccessMessage"] = "Expense updated successfully."; // Success message
        //        return RedirectToAction("ExpensesIndex"); // Redirect on success
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while updating the expense.");
        //        TempData["ErrorMessage"] = "An error occurred while processing your request."; // Error message for catch block
        //        return RedirectToAction("ExpensesIndex"); // Redirect even on error
        //    }
        //}

        public async Task<IActionResult> UpdateExpense(Expense updatedExpense)
        {
            try
            {
                if (updatedExpense == null || updatedExpense.ExpenseId == Guid.Empty)
                {
                    _logger.LogError("Expense or ExpenseId is null/empty.");
                    TempData["ErrorMessage"] = "Invalid Expense data.";
                    return RedirectToAction("ExpensesIndex"); // Adjust according to your routing
                }

                var filter = Builders<Expense>.Filter.Eq(e => e.ExpenseId, updatedExpense.ExpenseId);
                var update = Builders<Expense>.Update
                    .Set(e => e.Title, updatedExpense.Title)
                    .Set(e => e.Amount, updatedExpense.Amount)
                    .Set(e => e.Description, updatedExpense.Description)
                    .Set(e => e.ExpenseDate, updatedExpense.ExpenseDate)
                    .Set(e => e.CategoryId, updatedExpense.CategoryId);

                var result = await _mongoDbContext.Expenses.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    TempData["ErrorMessage"] = "Expense not found or could not be updated.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Expense updated successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the expense.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            return RedirectToAction("ExpensesIndex"); // Adjust according to your routing
        }



        //[HttpGet]
        //public async Task<IActionResult> GetExpenseDetails(Guid id)
        //{
        //    var expense = await _mongoDbContext.Expenses
        //        .Find(Builders<Expense>.Filter.Eq(e => e.ExpenseId, id))
        //        .FirstOrDefaultAsync();

        //    if (expense == null)
        //    {
        //        return NotFound();
        //    }

        //    return Json(new
        //    {
        //        title = expense.Title,
        //        amount = expense.Amount,
        //        description = expense.Description,
        //        date = expense.ExpenseDate.ToString("dd-MM-yyyy")
        //    });
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var filter = Builders<Expense>.Filter.Eq(c => c.ExpenseId, id);
            var result = await _mongoDbContext.Expenses.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return Json(new { success = false, message = "Expense not found" });
            }

            return Json(new { success = true });
        }




    }
}
