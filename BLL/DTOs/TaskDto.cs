using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public UserDto User { get; set; }
        public DateTime Deadline { get; set; }
        public IFormFile NewImage { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
