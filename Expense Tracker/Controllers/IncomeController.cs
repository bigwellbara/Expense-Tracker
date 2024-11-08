using Expense_Tracker.Models;
using Expense_Tracker.Models.Database;
using Expense_Tracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Diagnostics;

namespace Expense_Tracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ILogger<IncomeController> _logger;
        private readonly MongoDbContext _mongoDbContext;


        public IncomeController(MongoDbContext mongoDbContext, ILogger<IncomeController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }
        //public async Task<IActionResult> IncomeIndex()
        //{

        //    var allIncome = await _mongoDbContext.Incomes.Find(_ => true).ToListAsync();
        //    return View("~/Views/IncomeExpenses/IncomeIndex.cshtml", allIncome);

        //}

     
        public async Task<IActionResult> IncomeIndex()
        {
         
            var allIncomes = await _mongoDbContext.Incomes.Find(_ => true).ToListAsync();
            var incomeCategories = await _mongoDbContext.Categories.Find(c => c.Type == "Income").ToListAsync();
            var viewModel = new IncomeViewModel
            {
                Income = allIncomes,
                Categories = incomeCategories
            };

            return View("~/Views/IncomeExpenses/IncomeIndex.cshtml", viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> CreateIncome(Income income)
        {
            try
            {
              
                income.IncomeId = Guid.NewGuid();
                income.CreatedDate = DateTime.Now;
                await _mongoDbContext.Incomes.InsertOneAsync(income);

                // Redirect to the index view or show success message
                return RedirectToAction("IncomeIndex");
            }
            catch (Exception ex)
            {

                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var filter = Builders<Income>.Filter.Eq(c => c.IncomeId, id);
            var result = await _mongoDbContext.Incomes.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return Ok();
        }





        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }

        //    var income = await _mongoDbContext.Incomes
        //        .Find(c => c.IncomeId == id).FirstOrDefaultAsync();

        //    if (income == null)
        //    {
        //        return NotFound();
        //    }

        //    // Fetch categories where Type is Income
        //    var incomeCategories = await _mongoDbContext.Categories
        //        .Find(c => c.Type == "Income").ToListAsync();

        //    // Create a ViewModel for the partial view
        //    var viewModel = new IncomeEditViewModel
        //    {
        //        Income = income,
        //        Categories = incomeCategories
        //    };

        //    // Return the partial view
        //    return PartialView("~/Views/IncomeExpenses/_EditIncomePartial.cshtml", viewModel);
        //}


        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }

        //    var income = await _mongoDbContext.Incomes
        //        .Find(c => c.IncomeId == id).FirstOrDefaultAsync();

        //    if (income == null)
        //    {
        //        return NotFound();
        //    }
        //    var incomeCategories = await _mongoDbContext.Categories
        //        .Find(c => c.Type == "Income").ToListAsync();

        //    ViewBag.Categories = incomeCategories; // Store categories in ViewBag
        //    var viewModel = new IncomeEditViewModel
        //    {
        //        Income = income
        //    };

        //    // Return the partial view
        //    return PartialView("~/Views/IncomeExpenses/_EditIncomePartial.cshtml", viewModel);
        //}


        public async Task<IActionResult> Edit(Guid id)
        { 
            var income = await _mongoDbContext.Incomes.Find(e => e.IncomeId == id).FirstOrDefaultAsync();
            if (income == null)
            {
                return NotFound();
            }

            // Load categories for the dropdown
            ViewBag.Categories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();
            return PartialView("~/Views/IncomeExpenses/_EditIncomePartial.cshtml", income);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIncome(Income updatedIncome)
        {
            try
            {
                // Check if the updatedIncome object or its IncomeId is null/empty
                if (updatedIncome == null || updatedIncome.IncomeId == Guid.Empty)
                {
                    _logger.LogError("Income or IncomeId is null/empty.");
                    TempData["ErrorMessage"] = "Invalid Income data.";
                    return RedirectToAction("IncomeIndex"); // Adjust according to your routing
                }

                // Define the filter to find the specific income record to update
                var filter = Builders<Income>.Filter.Eq(i => i.IncomeId, updatedIncome.IncomeId);

                // Set the fields to update
                var update = Builders<Income>.Update
                    .Set(i => i.Source, updatedIncome.Source)
                    .Set(i => i.Amount, updatedIncome.Amount)
                    .Set(i => i.Description, updatedIncome.Description)
                    .Set(i => i.IncomeDate, updatedIncome.IncomeDate)
                    .Set(i => i.CategoryId, updatedIncome.CategoryId);

                // Perform the update operation
                var result = await _mongoDbContext.Incomes.UpdateOneAsync(filter, update);

                // Check if any document was modified
                if (result.ModifiedCount == 0)
                {
                    TempData["ErrorMessage"] = "Income not found or could not be updated.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Income updated successfully.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception and set an error message
                _logger.LogError(ex, "An error occurred while updating the income.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            // Redirect to the IncomeIndex view to show messages
            return RedirectToAction("IncomeIndex"); // Adjust according to your routing
        }





    }
}
