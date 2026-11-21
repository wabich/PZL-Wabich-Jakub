using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Domain;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services;

public class TaskService : ITaskService
{
    private readonly TaskFlowDbContext _db;
    public TaskService(TaskFlowDbContext db) => _db = db;

    private static readonly Expression<Func<TaskItem, TaskDto>> ToDto = t => new TaskDto
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status.ToString(),
        Priority = t.Priority.ToString(),
        CreatedAt = t.CreatedAt,
        DueDate = t.DueDate,
        ProjectId = t.ProjectId,
        ProjectName = t.Project.Name,
        AssigneeId = t.AssigneeId,
        AssigneeName = t.Assignee != null ? t.Assignee.FullName : null
    };

    public Task<List<TaskDto>> GetAllAsync() =>
        _db.Tasks.AsNoTracking().Select(ToDto).ToListAsync();

    public Task<TaskDto?> GetByIdAsync(int id) =>
        _db.Tasks.AsNoTracking().Where(t => t.Id == id).Select(ToDto).FirstOrDefaultAsync();

    public async Task<TaskDto?> CreateAsync(CreateTaskDto dto)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == dto.ProjectId))
            return null;

        if (dto.AssigneeId is not null &&
            !await _db.Users.AnyAsync(u => u.Id == dto.AssigneeId))
            return null;

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            AssigneeId = dto.AssigneeId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(task.Id);
    }
}