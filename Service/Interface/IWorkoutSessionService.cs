using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IWorkoutSessionService
{
    Task<List<WorkoutSession>> GetAllAsync();
    Task<WorkoutSession?> GetByIdAsync(Guid id);
    Task<WorkoutSession> GetByIdNotNullAsync(Guid id);
    Task<WorkoutSession> InsertAsync(WorkoutSessionDto workoutSessionDto);
    Task<WorkoutSession> UpdateAsync(Guid id, WorkoutSessionDto workoutSessionDto);
    Task<WorkoutSession> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<WorkoutSession>> GetAllPagedAsync(int pageNumber, int pageSize);
    
    Task<List<WorkoutSession>> GetAllByMemberNameAsync(Guid memberId);
    public Task<ICollection<WorkoutSession>> AddRangeAsync(List<WorkoutSession> ws);
}