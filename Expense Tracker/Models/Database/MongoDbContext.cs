using Expense_Tracker.Models.UserIdentity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Expense_Tracker.Models.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }


        //identity user
        public IMongoCollection<ApplicationUser> Users => _database.GetCollection<ApplicationUser>("Users");
        public IMongoCollection<ApplicationRole> Roles => _database.GetCollection<ApplicationRole>("Roles");


        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        //public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Expense> Expenses => _database.GetCollection<Expense>("Expenses");
        public IMongoCollection<Income> Incomes => _database.GetCollection<Income>("Incomes");
        public IMongoCollection<PaymentMethod> PaymentMethods => _database.GetCollection<PaymentMethod>("PaymentMethods");
        public IMongoCollection<RecurringExpense> RecurringExpenses => _database.GetCollection<RecurringExpense>("RecurringExpenses");
        public IMongoCollection<Budget> Budgets => _database.GetCollection<Budget>("Budgets");
    }
}
