using Expense_Tracker.Models;
using Expense_Tracker.Models.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using Expense_Tracker.ViewModels;

namespace Expense_Tracker.Controllers
{
    public class ExpensesController : Controller
    {

        private readonly MongoDbContext _mongoDbContext;

        public ExpensesController( MongoDbContext mongoDbContext)
        {
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



        public async Task<IActionResult> Edit(Guid id)
        {
            var expense = await _mongoDbContext.Expenses.Find(x => x.ExpenseId == id).FirstOrDefaultAsync();
            if (expense == null) return NotFound();

            var viewModel = new ExpenseEditViewModel
            {
                Expense = expense,
                Categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync()
            };

            return PartialView("~/Views/IncomeExpenses/_EditExpensePartial.cshtml", viewModel);

         
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateExpense(Guid id, Expense updatedExpense)
        //{
        //    if (id != updatedExpense.ExpenseId)
        //    {
        //        return Json(new { success = false, message = "Bad Request" });
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new { success = false, message = "Invalid model state" });
        //    }

        //    var filter = Builders<Expense>.Filter.Eq(e => e.ExpenseId, id);
        //    var update = Builders<Expense>.Update
        //        .Set(e => e.Title, updatedExpense.Title)
        //        .Set(e => e.Amount, updatedExpense.Amount)
        //        .Set(e => e.Description, updatedExpense.Description)
        //        .Set(e => e.ExpenseDate, updatedExpense.ExpenseDate)
        //        .Set(e => e.CategoryId, updatedExpense.CategoryId);

        //    var result = await _mongoDbContext.Expenses.UpdateOneAsync(filter, update);

        //    if (result.ModifiedCount == 0)
        //    {
        //        return Json(new { success = false, message = "Not Found" });
        //    }

        //    return Json(new { success = true });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateExpense(Guid id, ExpenseEditViewModel model)
        {
            if (id != model.Expense.ExpenseId)
            {
                // Redirect or return an error view if the IDs do not match
                ModelState.AddModelError(string.Empty, "Bad Request");
                return View(model); // Return the model with errors to the view
            }

            if (!ModelState.IsValid)
            {
                // Return the view with the model to display validation errors
                return View(model);
            }

            var filter = Builders<Expense>.Filter.Eq(e => e.ExpenseId, id);
            var update = Builders<Expense>.Update
                .Set(e => e.Title, model.Expense.Title)
                .Set(e => e.Amount, model.Expense.Amount)
                .Set(e => e.Description, model.Expense.Description)
                .Set(e => e.ExpenseDate, model.Expense.ExpenseDate)
                .Set(e => e.CategoryId, model.Expense.CategoryId);

            var result = await _mongoDbContext.Expenses.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                ModelState.AddModelError(string.Empty, "Not Found");
                return View(model); // Return the model with errors to the view
            }

            // Redirect to an appropriate action after a successful update
            return RedirectToAction("ExpensesIndex"); // Change "Index" to your desired action
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
