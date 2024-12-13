using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using AspNetCore.Identity.Mongo.Model;

namespace Expense_Tracker.Models.UserIdentity
{
    public class ApplicationUser :MongoUser
    {
      

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("NationalID")]
        public string NationalID { get; set; }
        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [BsonElement("UpdatedDate")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

    }
}
