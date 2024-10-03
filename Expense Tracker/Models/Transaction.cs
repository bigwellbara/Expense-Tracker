using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [BsonId]  // Map _id to TransactionId
        [BsonRepresentation(BsonType.String)]  // Store the ID as a string (optional)
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]  // The transaction amount (can be for either expense or income)
        public decimal Amount { get; set; }

        [BsonElement("TransactionType")]  // Either 'Expense' or 'Income'
        public string TransactionType { get; set; }

        [BsonElement("TransactionDate")]  
        public DateTime TransactionDate { get; set; }

        [BsonElement("UserId")]  // Foreign key reference to User
        [BsonRepresentation(BsonType.String)]  
        public Guid UserId { get; set; }

        [BsonElement("ExpenseId")]  // Nullable foreign key reference to Expense
        [BsonRepresentation(BsonType.String)]
        public Guid? ExpenseId { get; set; }  // Nullable since not all transactions are expenses

        [BsonElement("IncomeId")]  // Nullable foreign key reference to Income
        [BsonRepresentation(BsonType.String)]
        public Guid? IncomeId { get; set; }  // Nullable since not all transactions are income

        [BsonElement("PaymentMethodId")]  // Foreign key reference to PaymentMethod
        [BsonRepresentation(BsonType.String)]  
        public Guid PaymentMethodId { get; set; }

        [BsonElement("CreatedDate")]  // Automatically set the creation date
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
