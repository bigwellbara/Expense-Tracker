using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Expense_Tracker.Models
{
    public class RecurringExpense
    {
        [BsonId]  // Map _id to RecurringExpenseId
        [BsonRepresentation(BsonType.String)]  // Store the ID as a string (optional)
        public Guid RecurringExpenseId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]  
        public decimal Amount { get; set; }

        [BsonElement("Title")] 
        public string Title { get; set; }

        [BsonElement("Description")]  
        public string Description { get; set; } = "";

        [BsonElement("StartDate")]  // Start date for the recurring expense
        public DateTime StartDate { get; set; }

        [BsonElement("Frequency")]  // Frequency of the recurring expense (e.g., Monthly, Weekly)
        public string Frequency { get; set; }

        [BsonElement("UserId")]  // Foreign key reference to User
        [BsonRepresentation(BsonType.String)]  // Store as a string (you can use ObjectId if preferred)
        public Guid UserId { get; set; }

        [BsonElement("CategoryId")]  // Foreign key reference to Category
        [BsonRepresentation(BsonType.String)]  // Store as a string (you can use ObjectId if preferred)
        public Guid CategoryId { get; set; }

        [BsonElement("CreatedDate")]  // Automatically set the creation date
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
