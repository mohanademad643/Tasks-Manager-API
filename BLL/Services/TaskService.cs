﻿

using AutoMapper;
using BLL.DTOs;
using BLL.Iservices;
using DAL.Identity;
using DAL.IRepository;
using DAL.Models.Entites;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webhostenvironment;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager,IWebHostEnvironment webhostenvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager; 
            _webhostenvironment = webhostenvironment;
        }


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var Users = await _unitOfWork.Users.GetAllUserAsync();
                return _mapper.Map<IEnumerable<UserDto>>(Users);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetAllAsync();
                return _mapper.Map<IEnumerable<TaskDto>>(tasks);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all tasks.", ex);
            }
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Task with ID {id} not found.");
                }
                return _mapper.Map<TaskDto>(task);
            }
            catch (KeyNotFoundException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving task with ID {id}.", ex);
            }
        }
        public async Task<TaskDto> UpdateTaskStatusAsync(int taskId, string userId, string newStatus)
        {
            var task = await _unitOfWork.Tasks.UpdateTaskStatusAsync(taskId, userId, newStatus);
            if (task == null)
            {
                return null;
            }

            return new TaskDto
            {
                Id = (task as Tasks).Id,
                Title = task.Title,
                User = new UserDto
                {
                    Id = task.User.Id,
                    Username = task.User.UserName,
                    Status = task.User.Status,
                    Role = task.User.Role
                },
                Deadline = task.Deadline ?? DateTime.Now,
                Image = (task as Tasks).Image,
                Description = (task as Tasks).Description,
                Status = (task as Tasks).Status
            };
        }
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            try
            {
                if (createTaskDto == null)
                {
                    throw new ArgumentNullException(nameof(createTaskDto));
                }
                ;
                var Filename = "";
                if (createTaskDto.NewImage != null)
                {
                    string FullPath = Path.Combine(_webhostenvironment.WebRootPath,
                        "uploads");
                    if (!Directory.Exists(FullPath))
                    {
                        Directory.CreateDirectory(FullPath);
                    }

                    Filename = Guid.NewGuid() + "_" + createTaskDto.NewImage.FileName;

                    string ImagePath = Path.Combine(FullPath, Filename);

                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        createTaskDto.NewImage.CopyToAsync(stream);
                        stream.Dispose();
                    }
                }
                var AddTaskDto = new CreateTaskDto()
                {
                    Title = createTaskDto.Title,
                    UserId = createTaskDto.UserId,
                    Image = Filename,
                    Description = createTaskDto.Description,
                    Status = createTaskDto.Status
                };
                var task = _mapper.Map<Tasks>(AddTaskDto);

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(createTaskDto.UserId))
                {
                    var user = await _userManager.FindByIdAsync(createTaskDto.UserId);
                    if (user != null)
                    {
                        user.TotalTasks += 1;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            throw new InvalidOperationException("Failed to update user's total tasks.");
                        }
                    }
                }

                return _mapper.Map<TaskDto>(task);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the task.", ex);
            }
        }
        public async Task<IEnumerable<TaskDto>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate) 
        {
            var tasks = await _unitOfWork.Tasks.GetAllByDateRangeAsync(startDate, endDate);
            return tasks.Select(t => new TaskDto
            {
                Id = (t as Tasks)?.Id ?? 0,
                Title = t.Title,
                User = new UserDto
                {
                    Id = t.User.Id,
                    Username = t.User.UserName,
                    Status = t.User.Status,
                    Role = t.User.Role
                },
                Deadline = t.Deadline ?? DateTime.Now,
                Image = t.Image,
                Description = t.Description,
                Status = t.Status
            }).ToList();
        }
        public async Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (updateTaskDto == null)
                {
                    throw new ArgumentNullException(nameof(updateTaskDto));
                }

                var task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Task with ID {id} not found.");
                }

                string? imagePath = task.Image;
                if (updateTaskDto.NewImage != null && updateTaskDto.NewImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webhostenvironment.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + updateTaskDto.NewImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateTaskDto.NewImage.CopyToAsync(fileStream);
                    }

                   
                    imagePath = $"/Uploads/{uniqueFileName}";
           
                    if (!string.IsNullOrEmpty(task.Image))
                    {
                        var oldImagePath = Path.Combine(_webhostenvironment.WebRootPath, task.Image.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                }

               
                _mapper.Map(updateTaskDto, task);
                task.Image = imagePath; 

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (KeyNotFoundException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                
                throw new ApplicationException($"An error occurred while updating task with ID {id}.", ex);
            }
        }
        public async Task<IEnumerable<TaskDto>> SearchTasksByTitleAsync(string title)
        {
            var tasks = await _unitOfWork.Tasks.GetAllTasksByTitle(title);
            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                User = new UserDto
                {
                    Id = t.User.Id,
                    Username = t.User.UserName,
                    Status = t.User.Status,
                    Role = t.User.Role
                },
                Deadline = t.Deadline ?? DateTime.Now,
                Image = t.Image,
                Description = t.Description,
                Status = t.Status
            }).ToList();
        }
        public async Task<bool> UpdateUserStatusAsync(string userId, string status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Status = status;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task DeleteTaskAsync(int id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Task with ID {id} not found.");
                }

                _unitOfWork.Tasks.Delete(task);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(task.UserId))
                {
                    var user = await _userManager.FindByIdAsync(task.UserId);
                    if (user != null && user.TotalTasks > 0)
                    {
                        user.TotalTasks--;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            throw new InvalidOperationException("Failed to update user's total tasks.");
                        }
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting task with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                var tasks = (await _unitOfWork.Tasks.GetAllAsync()).Where(t => t.UserId == userId);
                return _mapper.Map<IEnumerable<TaskDto>>(tasks);
            }
            catch (ArgumentNullException)
            {
                throw; 
            }
            catch (Exception ex)
            {

                throw new ApplicationException($"An error occurred while retrieving tasks for user ID {userId}.", ex);
            }
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    throw new ArgumentNullException(nameof(status));
                }

                var tasks = (await _unitOfWork.Tasks.GetAllAsync()).Where(t => t.Status == status);
                return _mapper.Map<IEnumerable<TaskDto>>(tasks);
            }
            catch (ArgumentNullException)
            {
                throw; 
            }
            catch (Exception ex)
            {
             
                throw new ApplicationException($"An error occurred while retrieving tasks with status {status}.", ex);
            }
        }
    }
}