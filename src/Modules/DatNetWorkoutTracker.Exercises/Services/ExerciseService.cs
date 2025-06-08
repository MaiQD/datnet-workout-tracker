using DatNetWorkoutTracker.Shared.Domain;
using DatNetWorkoutTracker.Exercises.Domain;

namespace DatNetWorkoutTracker.Exercises.Services;

public class ExerciseService : IExerciseService
{
    private readonly IRepository<Exercise> _exerciseRepository;

    public ExerciseService(IRepository<Exercise> exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<Exercise?> GetExerciseByIdAsync(string exerciseId)
    {
        return await _exerciseRepository.GetByIdAsync(exerciseId);
    }

    public async Task<IEnumerable<Exercise>> GetAllExercisesAsync()
    {
        return await _exerciseRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(ExerciseCategory category)
    {
        return await _exerciseRepository.FindAsync(e => e.Category == category);
    }

    public async Task<IEnumerable<Exercise>> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        return await _exerciseRepository.FindAsync(e => e.TargetMuscleGroups.Contains(muscleGroup));
    }

    public async Task<IEnumerable<Exercise>> GetExercisesByEquipmentAsync(Equipment equipment)
    {
        return await _exerciseRepository.FindAsync(e => e.Equipment.Contains(equipment));
    }

    public async Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _exerciseRepository.FindAsync(e => 
            e.Name.ToLower().Contains(lowerSearchTerm) ||
            e.Description.ToLower().Contains(lowerSearchTerm) ||
            e.Tags.Any(tag => tag.ToLower().Contains(lowerSearchTerm)));
    }

    public async Task<IEnumerable<Exercise>> GetCustomExercisesByUserAsync(string userId)
    {
        return await _exerciseRepository.FindAsync(e => e.IsCustom && e.CreatedByUserId == userId);
    }

    public async Task<Exercise> CreateExerciseAsync(Exercise exercise)
    {
        return await _exerciseRepository.CreateAsync(exercise);
    }

    public async Task<Exercise> UpdateExerciseAsync(Exercise exercise)
    {
        return await _exerciseRepository.UpdateAsync(exercise);
    }

    public async Task DeleteExerciseAsync(string exerciseId)
    {
        await _exerciseRepository.DeleteAsync(exerciseId);
    }
}
