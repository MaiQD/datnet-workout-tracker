using DatNetWorkoutTracker.Routines.Domain;

namespace DatNetWorkoutTracker.Routines.Services;

public interface IRoutineService
{
    Task<Routine?> GetRoutineByIdAsync(string routineId);
    Task<IEnumerable<Routine>> GetRoutinesByUserAsync(string userId);
    Task<IEnumerable<Routine>> GetPublicRoutinesAsync();
    Task<IEnumerable<Routine>> SearchRoutinesAsync(string searchTerm);
    Task<IEnumerable<Routine>> GetRoutinesByTagAsync(string tag);
    Task<Routine> CreateRoutineAsync(Routine routine);
    Task<Routine> UpdateRoutineAsync(Routine routine);
    Task DeleteRoutineAsync(string routineId);
    Task<Routine> DuplicateRoutineAsync(string routineId, string newUserId);
    Task MarkRoutineAsUsedAsync(string routineId);
}

public class CreateWorkoutFromRoutineRequest
{
    public string RoutineId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string? WorkoutName { get; set; }
}
