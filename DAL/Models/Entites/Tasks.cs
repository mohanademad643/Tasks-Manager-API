

using DAL.Identity;

namespace DAL.Models.Entites
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime? Deadline { get; set; } = DateTime.Now;
        public string Image { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }


    }
}
