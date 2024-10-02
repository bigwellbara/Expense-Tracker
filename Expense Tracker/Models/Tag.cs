using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class Tag
    {
        [Key] 
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; } 
    }
}
