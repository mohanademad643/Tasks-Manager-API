using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class CreateTaskDto
    {
        [Required] public string Title { get; set; }
        public string UserId { get; set; }
         public DateTime? Deadline { get; set; } = DateTime.Now;
        public IFormFile NewImage { get; set; }
        public string? Image { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string Status { get; set; } = "In-Progress";
    }
}
