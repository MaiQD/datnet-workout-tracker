using DatNetWorkoutTracker.Exercises.Domain;

namespace DatNetWorkoutTracker.Exercises.Services;

public interface IExerciseService
{
    Task<Exercise?> GetExerciseByIdAsync(string exerciseId);
    Task<IEnumerable<Exercise>> GetAllExercisesAsync();
    Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(ExerciseCategory category);
    Task<IEnumerable<Exercise>> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup);
    Task<IEnumerable<Exercise>> GetExercisesByEquipmentAsync(Equipment equipment);
    Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm);
    Task<IEnumerable<Exercise>> GetCustomExercisesByUserAsync(string userId);
    Task<Exercise> CreateExerciseAsync(Exercise exercise);
    Task<Exercise> UpdateExerciseAsync(Exercise exercise);
    Task DeleteExerciseAsync(string exerciseId);
}
