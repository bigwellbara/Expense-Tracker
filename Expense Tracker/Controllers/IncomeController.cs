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

        private readonly MongoDbContext _mongoDbContext;


        public IncomeController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
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


   


        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var income = await _mongoDbContext.Incomes
                .Find(c => c.IncomeId == id).FirstOrDefaultAsync();

            if (income == null)
            {
                return NotFound();
            }

            // Fetch categories where Type is Income
            var incomeCategories = await _mongoDbContext.Categories
                .Find(c => c.Type == "Income").ToListAsync();

            // Create a ViewModel for the partial view
            var viewModel = new IncomeEditViewModel
            {
                Income = income,
                Categories = incomeCategories
            };

            // Return the partial view
            return PartialView("~/Views/IncomeExpenses/_EditIncomePartial.cshtml", viewModel);
        }


 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIncome(Guid id, Income updatedIncome)
        {
            if (id != updatedIncome.IncomeId)
            {
                return Json(new { success = false, message = "Bad Request" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid model state" });
            }

            var filter = Builders<Income>.Filter.Eq(c => c.IncomeId, id);

            var update = Builders<Income>.Update
                .Set(c => c.Source, updatedIncome.Source)
                .Set(c => c.Amount, updatedIncome.Amount)
                .Set(c => c.Description, updatedIncome.Description)
                .Set(c => c.IncomeDate, updatedIncome.IncomeDate)
                .Set(c => c.CategoryId, updatedIncome.CategoryId);

            var result = await _mongoDbContext.Incomes.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0) 
            {
                return Json(new { success = false, message = "Not Found" });
            }

            return Json(new { success = true });
        }




    }
}
