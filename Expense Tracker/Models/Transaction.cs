using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [BsonId]  // Map _id to TransactionId
        [BsonRepresentation(BsonType.String)]  
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]  
        public decimal Amount { get; set; }

        [BsonElement("TransactionType")]  
        public string TransactionType { get; set; }

        [BsonElement("TransactionDate")]  
        public DateTime TransactionDate { get; set; }

        [BsonElement("UserId")]  
        [BsonRepresentation(BsonType.String)]  
        public Guid UserId { get; set; }

        [BsonElement("ExpenseId")]  
        [BsonRepresentation(BsonType.String)]
        public Guid? ExpenseId { get; set; }  

        [BsonElement("IncomeId")]  
        [BsonRepresentation(BsonType.String)]
        public Guid? IncomeId { get; set; }  

        [BsonElement("PaymentMethodId")]  
        [BsonRepresentation(BsonType.String)]  
        public Guid PaymentMethodId { get; set; }

        [BsonElement("CreatedDate")] 
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
