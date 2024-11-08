using Expense_Tracker.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    public class ReportsController :Controller
    {

        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<ReportsController> _logger;
        ReportsController(MongoDbContext mongoDbContext, ILogger<ReportsController> logger) 
        { 
        _mongoDbContext = mongoDbContext;
         _logger = logger;
        }



    }
}
