using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Expense_Tracker.Models
{
    public class Budget
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid BudgetId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]
        public decimal Amount { get; set; }

        [BsonElement("StartDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [BsonElement("CategoryId")]
        [BsonRepresentation(BsonType.String)]
        public Guid CategoryId { get; set; }

        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Additional fields for budget tracking
        [BsonElement("IsExceeded")]
        public bool IsExceeded { get; set; } = false; // Indicates if the budget limit has been exceeded

        [BsonElement("RemainingAmount")]
        public decimal RemainingAmount { get; set; } // To track how much is left within the budget

        [BsonElement("AlertThreshold")]
        public double AlertThreshold { get; set; } = 80.0; // Alert at 80% by default

        [BsonElement("Notes")]
        public string Notes { get; set; } // Optional notes about the budget
    }


//    Setting Budgets: Create a view for users to input budgets for each category.

//Tracking Spending: In the TransactionsController, update the logic to track how much of each budget has been used by summing up transactions within the budget period and category.

//Notifications and Alerts: Add logic to check if spending is nearing or has exceeded the budgeted amount, which can trigger alerts or notifications in the UI.

//Budget Summary: Provide a summary view showing budgets, spending, and remaining amounts for each category.



}
