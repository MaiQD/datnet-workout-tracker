using DatNetWorkoutTracker.Shared.Domain;
using DatNetWorkoutTracker.Workouts.Domain;

namespace DatNetWorkoutTracker.Workouts.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IRepository<Workout> _workoutRepository;

    public WorkoutService(IRepository<Workout> workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    public async Task<Workout?> GetWorkoutByIdAsync(string workoutId)
    {
        return await _workoutRepository.GetByIdAsync(workoutId);
    }

    public async Task<IEnumerable<Workout>> GetWorkoutsByUserAsync(string userId)
    {
        return await _workoutRepository.FindAsync(w => w.UserId == userId);
    }

    public async Task<IEnumerable<Workout>> GetWorkoutsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
    {
        return await _workoutRepository.FindAsync(w => 
            w.UserId == userId && 
            w.StartTime >= startDate && 
            w.StartTime <= endDate);
    }

    public async Task<IEnumerable<Workout>> GetWorkoutsByStatusAsync(string userId, WorkoutStatus status)
    {
        return await _workoutRepository.FindAsync(w => w.UserId == userId && w.Status == status);
    }

    public async Task<Workout?> GetActiveWorkoutAsync(string userId)
    {
        var activeWorkouts = await _workoutRepository.FindAsync(w => 
            w.UserId == userId && w.Status == WorkoutStatus.InProgress);
        return activeWorkouts.FirstOrDefault();
    }

    public async Task<Workout> CreateWorkoutAsync(Workout workout)
    {
        return await _workoutRepository.CreateAsync(workout);
    }

    public async Task<Workout> UpdateWorkoutAsync(Workout workout)
    {
        return await _workoutRepository.UpdateAsync(workout);
    }

    public async Task<Workout> StartWorkoutAsync(string workoutId)
    {
        var workout = await _workoutRepository.GetByIdAsync(workoutId);
        if (workout != null)
        {
            workout.Status = WorkoutStatus.InProgress;
            workout.StartTime = DateTime.UtcNow;
            await _workoutRepository.UpdateAsync(workout);
        }
        return workout!;
    }

    public async Task<Workout> CompleteWorkoutAsync(string workoutId)
    {
        var workout = await _workoutRepository.GetByIdAsync(workoutId);
        if (workout != null)
        {
            workout.Status = WorkoutStatus.Completed;
            workout.EndTime = DateTime.UtcNow;
            workout.Duration = workout.EndTime - workout.StartTime;
            await _workoutRepository.UpdateAsync(workout);
        }
        return workout!;
    }

    public async Task<Workout> CancelWorkoutAsync(string workoutId)
    {
        var workout = await _workoutRepository.GetByIdAsync(workoutId);
        if (workout != null)
        {
            workout.Status = WorkoutStatus.Cancelled;
            await _workoutRepository.UpdateAsync(workout);
        }
        return workout!;
    }

    public async Task DeleteWorkoutAsync(string workoutId)
    {
        await _workoutRepository.DeleteAsync(workoutId);
    }

    public async Task<WorkoutStatistics> GetWorkoutStatisticsAsync(string userId, DateTime? fromDate = null)
    {
        var workouts = await _workoutRepository.FindAsync(w => 
            w.UserId == userId && 
            w.Status == WorkoutStatus.Completed &&
            (fromDate == null || w.StartTime >= fromDate));

        var stats = new WorkoutStatistics
        {
            TotalWorkouts = workouts.Count(),
            TotalWorkoutTime = TimeSpan.FromTicks(workouts.Sum(w => w.Duration?.Ticks ?? 0)),
            TotalSets = workouts.Sum(w => w.Exercises.Sum(e => e.Sets.Count)),
            TotalReps = workouts.Sum(w => w.Exercises.Sum(e => e.Sets.Sum(s => s.Reps ?? 0))),
            TotalWeightLifted = workouts.Sum(w => w.Exercises.Sum(e => e.Sets.Sum(s => (s.Weight ?? 0) * (s.Reps ?? 0))))
        };

        // Exercise frequency
        foreach (var workout in workouts)
        {
            foreach (var exercise in workout.Exercises)
            {
                if (stats.ExerciseFrequency.ContainsKey(exercise.ExerciseName))
                    stats.ExerciseFrequency[exercise.ExerciseName]++;
                else
                    stats.ExerciseFrequency[exercise.ExerciseName] = 1;
            }
        }

        // Workouts by day of week
        foreach (var workout in workouts)
        {
            var dayOfWeek = workout.StartTime.DayOfWeek;
            if (stats.WorkoutsByDayOfWeek.ContainsKey(dayOfWeek))
                stats.WorkoutsByDayOfWeek[dayOfWeek]++;
            else
                stats.WorkoutsByDayOfWeek[dayOfWeek] = 1;
        }

        return stats;
    }
}
