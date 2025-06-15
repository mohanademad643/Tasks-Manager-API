
using DAL.Models.Entites;
using Microsoft.AspNetCore.Identity;

namespace DAL.Identity
{
    public class User: IdentityUser
    {
        public string Status { get; set; }
        public string Role { get; set; }
        public int TotalTasks { get; set; }
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    }
    
}
