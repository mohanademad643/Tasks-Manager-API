using Microsoft.AspNetCore.Http;


namespace BLL.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string UserId { get; set; }
        public DateTime? Deadline { get; set; }
        public IFormFile NewImage { get; set; }
        public string? Image { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
