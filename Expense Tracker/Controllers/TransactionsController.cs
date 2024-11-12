using Expense_Tracker.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(MongoDbContext mongoDbContext, ILogger<TransactionsController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }
    }
}
