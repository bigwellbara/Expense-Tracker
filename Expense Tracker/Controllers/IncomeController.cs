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


    }
}
