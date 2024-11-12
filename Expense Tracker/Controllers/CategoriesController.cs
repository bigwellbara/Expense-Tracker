using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using Expense_Tracker.Models.Database;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Expense_Tracker.Controllers
{
    public class CategoriesController : Controller
    {
        //private readonly ApplicationDbContext _context;

        private readonly MongoDbContext _mongoDbContext;

        public CategoriesController( MongoDbContext mongoDbContext)
        {
            //_context = context;
            _mongoDbContext = mongoDbContext;
        }

     
        public async Task<IActionResult> Index()
        {
            //Using database db context
            //return View(await _context.Categories.ToListAsync());

            // using mongodb context
            
            var allCategories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync(); 
            return View(allCategories); 
        }

        public async Task<IActionResult> CategoriesDashboard()
        {
          
            // using mongodb context

            var allCategories = await _mongoDbContext.Categories.Find(_ => true).ToListAsync();
            return View(allCategories);
        }





        //[HttpGet]
        //public IActionResult Filters(List<string> types)
        //{

        //    var categoriesCollection = _mongoDbContext.Categories.AsQueryable();  

        //    // Apply filtering based on the types (Expense or Income)
        //    if (types != null && types.Count > 0)
        //    {
        //        categoriesCollection = categoriesCollection.Where(c => types.Contains(c.Type));
        //    }

        //    var filteredCategories = categoriesCollection.ToList();
        //    if (!filteredCategories.Any())
        //    {
        //        Console.WriteLine("No categories found for the selected types.");
        //    }
        //    return PartialView("~/Views/Categories/Partials/_CategoryTableRows.cshtml", filteredCategories);
        //}


        //[HttpGet]
        //public IActionResult Filters(List<string> types)
        //{
        //    var filter = Builders<Category>.Filter.Empty;

        //    if (types?.Any() == true)
        //    {
        //        filter = Builders<Category>.Filter.In(c => c.Type, types);
        //    }

        //    var filteredCategories = _mongoDbContext.Categories.Find(filter).ToList();



        //    return PartialView("~/Views/Categories/Partials/_CategoryTableRows.cshtml", filteredCategories);
        //}


        // POST: /Category/Filter
        [HttpPost]
        public IActionResult Filter([FromBody] FilterRequest filterRequest)
        {
            var filter = Builders<Category>.Filter.In(c => c.Type, filterRequest.Types);
            var filteredCategories = _mongoDbContext.Categories.Find(filter).ToList();

            return PartialView("_CategoryTablePartial", filteredCategories); // Use a partial view for the table content

     
        }


        public async Task<IActionResult> Details(Guid id)
        {
            

            //using mongo 

            // Find the category by CategoryId (which is now a GUID)
            var category = await _mongoDbContext.Categories
                .Find(category => category.CategoryId == id).FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        
        public IActionResult Create()
        {
            return View(new Category());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Title,Description,Type")] Category category)
        {
            

            if (ModelState.IsValid)
            {
             
                if (category.CategoryId == Guid.Empty)
                {
                    category.CategoryId = Guid.NewGuid();
                }

                await _mongoDbContext.Categories.InsertOneAsync(category);

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
           

            // using mongo db
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var category = await _mongoDbContext.Categories
                .Find(c => c.CategoryId == id).FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return View(category);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CategoryId,Title,Description,Type")] Category category)
        {          

            //using mongo db
            // Check if the ID in the route matches the ID in the model
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create a filter to find the category by its ID
                    var filter = Builders<Category>.Filter.Eq(c => c.CategoryId, id);
                    // Update the category document
                    var update = Builders<Category>.Update
                        .Set(c => c.Title, category.Title)
                        .Set(c => c.Description, category.Description)
                        .Set(c => c.Type, category.Type)         
                        .Set(c => c.CreatedDate, category.CreatedDate); // Ensure all fields are updated

                    var result = await _mongoDbContext.Categories.UpdateOneAsync(filter, update);

                    if (result.MatchedCount == 0)
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                 
                    Console.WriteLine(ex.Message);
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }


        public async Task<IActionResult> Delete(Guid id)
        {
       
            //using mongo
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var category = await _mongoDbContext.Categories
                .Find(c => c.CategoryId == id).FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
           
            //using mongo db
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            // Create a filter to find the category by its ID
            var filter = Builders<Category>.Filter.Eq(c => c.CategoryId, id);

            // Delete the category document
            var result = await _mongoDbContext.Categories.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

      

        //using mongo db
        private async Task<bool> CategoryExists(Guid id)
        {
            // Create a filter to find the category by its ID
            var filter = Builders<Category>.Filter.Eq(c => c.CategoryId, id);
            var count = await _mongoDbContext.Categories.CountDocumentsAsync(filter);

            return count > 0;
        }

    }
}


public class FilterRequest
{
    public List<string> Types { get; set; }
}
