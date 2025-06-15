using AutoMapper;
using BLL.DTOs;
using DAL.Identity;
using DAL.Models.Entites;

namespace BLL.AutoMapper
{
   
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<Tasks, TaskDto>();
            CreateMap<CreateTaskDto, Tasks>();
            CreateMap<UpdateTaskDto, Tasks>();
            CreateMap<User, UserDto>();
        }
    }
}
