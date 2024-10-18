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


    }
}
