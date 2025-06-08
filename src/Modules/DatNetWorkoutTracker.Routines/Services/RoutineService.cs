using DatNetWorkoutTracker.Shared.Domain;
using DatNetWorkoutTracker.Routines.Domain;

namespace DatNetWorkoutTracker.Routines.Services;

public class RoutineService : IRoutineService
{
    private readonly IRepository<Routine> _routineRepository;

    public RoutineService(IRepository<Routine> routineRepository)
    {
        _routineRepository = routineRepository;
    }

    public async Task<Routine?> GetRoutineByIdAsync(string routineId)
    {
        return await _routineRepository.GetByIdAsync(routineId);
    }

    public async Task<IEnumerable<Routine>> GetRoutinesByUserAsync(string userId)
    {
        return await _routineRepository.FindAsync(r => r.UserId == userId);
    }

    public async Task<IEnumerable<Routine>> GetPublicRoutinesAsync()
    {
        return await _routineRepository.FindAsync(r => r.IsPublic);
    }

    public async Task<IEnumerable<Routine>> SearchRoutinesAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _routineRepository.FindAsync(r => 
            r.Name.ToLower().Contains(lowerSearchTerm) ||
            (r.Description != null && r.Description.ToLower().Contains(lowerSearchTerm)) ||
            r.Tags.Any(tag => tag.ToLower().Contains(lowerSearchTerm)));
    }

    public async Task<IEnumerable<Routine>> GetRoutinesByTagAsync(string tag)
    {
        return await _routineRepository.FindAsync(r => r.Tags.Contains(tag));
    }

    public async Task<Routine> CreateRoutineAsync(Routine routine)
    {
        return await _routineRepository.CreateAsync(routine);
    }

    public async Task<Routine> UpdateRoutineAsync(Routine routine)
    {
        return await _routineRepository.UpdateAsync(routine);
    }

    public async Task DeleteRoutineAsync(string routineId)
    {
        await _routineRepository.DeleteAsync(routineId);
    }

    public async Task<Routine> DuplicateRoutineAsync(string routineId, string newUserId)
    {
        var originalRoutine = await _routineRepository.GetByIdAsync(routineId);
        if (originalRoutine == null)
            throw new ArgumentException("Routine not found", nameof(routineId));

        var duplicatedRoutine = new Routine
        {
            UserId = newUserId,
            Name = $"{originalRoutine.Name} (Copy)",
            Description = originalRoutine.Description,
            Exercises = originalRoutine.Exercises,
            Tags = originalRoutine.Tags,
            EstimatedDuration = originalRoutine.EstimatedDuration,
            Difficulty = originalRoutine.Difficulty,
            IsPublic = false // Duplicated routines are private by default
        };

        return await _routineRepository.CreateAsync(duplicatedRoutine);
    }

    public async Task MarkRoutineAsUsedAsync(string routineId)
    {
        var routine = await _routineRepository.GetByIdAsync(routineId);
        if (routine != null)
        {
            routine.TimesUsed++;
            routine.LastUsed = DateTime.UtcNow;
            await _routineRepository.UpdateAsync(routine);
        }
    }
}
