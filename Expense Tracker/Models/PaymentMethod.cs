using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Expense_Tracker.Models
{
    public class PaymentMethod
    {
        [BsonId]  // Map _id to PaymentMethodId
        [BsonRepresentation(BsonType.String)]  
        public Guid PaymentMethodId { get; set; } = Guid.NewGuid();

        [BsonElement("MethodName")]  // Name of the payment method (e.g., Cash, Credit Card)
        public string MethodName { get; set; }

        [BsonElement("UserId")]  // Foreign key reference to User
        [BsonRepresentation(BsonType.String)]  // Store as a string (you can use ObjectId if preferred)
        public Guid UserId { get; set; }

        [BsonElement("CreatedDate")]  // Automatically set the creation date
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
