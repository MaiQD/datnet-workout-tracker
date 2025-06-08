using DatNetWorkoutTracker.Workouts.Services;
using DatNetWorkoutTracker.Workouts.Domain;

namespace DatNetWorkoutTracker.Analytics.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IWorkoutService _workoutService;

    public AnalyticsService(IWorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    public async Task<ProgressReport> GetProgressReportAsync(string userId, DateTime fromDate, DateTime toDate)
    {
        var statistics = await _workoutService.GetWorkoutStatisticsAsync(userId, fromDate);
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, fromDate, toDate);

        var report = new ProgressReport
        {
            Statistics = statistics,
            WorkoutFrequency = (await GetWorkoutFrequencyAsync(userId, toDate - fromDate)).ToList(),
            PersonalRecords = await GetPersonalRecordsAsync(userId)
        };

        // Get strength progress for top exercises
        var topExercises = statistics.ExerciseFrequency
            .OrderByDescending(x => x.Value)
            .Take(5)
            .Select(x => x.Key);

        foreach (var exerciseName in topExercises)
        {
            var exerciseWorkouts = workouts.Where(w => w.Exercises.Any(e => e.ExerciseName == exerciseName));
            if (exerciseWorkouts.Any())
            {
                var exerciseId = exerciseWorkouts.First().Exercises.First(e => e.ExerciseName == exerciseName).ExerciseId;
                report.StrengthProgress.Add(await GetStrengthProgressAsync(userId, exerciseId, fromDate, toDate));
            }
        }

        return report;
    }

    public async Task<StrengthProgressData> GetStrengthProgressAsync(string userId, string exerciseId, DateTime fromDate, DateTime toDate)
    {
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, fromDate, toDate);
        var exerciseWorkouts = workouts
            .Where(w => w.Status == WorkoutStatus.Completed && w.Exercises.Any(e => e.ExerciseId == exerciseId))
            .OrderBy(w => w.StartTime);

        var dataPoints = new List<StrengthDataPoint>();
        var exerciseName = "";

        foreach (var workout in exerciseWorkouts)
        {
            var exercise = workout.Exercises.First(e => e.ExerciseId == exerciseId);
            exerciseName = exercise.ExerciseName;

            var weightSets = exercise.Sets.Where(s => s.Weight.HasValue && s.Reps.HasValue && s.IsCompleted);
            if (weightSets.Any())
            {
                var maxWeight = weightSets.Max(s => s.Weight!.Value);
                var totalVolume = weightSets.Sum(s => s.Weight!.Value * s.Reps!.Value);
                var bestSet = weightSets.OrderByDescending(s => s.Weight!.Value * s.Reps!.Value).First();
                var oneRepMax = CalculateOneRepMax(bestSet.Weight!.Value, bestSet.Reps!.Value);

                dataPoints.Add(new StrengthDataPoint
                {
                    Date = workout.StartTime,
                    MaxWeight = maxWeight,
                    TotalVolume = totalVolume,
                    OneRepMax = oneRepMax
                });
            }
        }

        return new StrengthProgressData
        {
            ExerciseId = exerciseId,
            ExerciseName = exerciseName,
            DataPoints = dataPoints
        };
    }

    public async Task<IEnumerable<WorkoutFrequencyData>> GetWorkoutFrequencyAsync(string userId, TimeSpan period)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate - period;
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, startDate, endDate);

        var frequencyData = workouts
            .Where(w => w.Status == WorkoutStatus.Completed)
            .GroupBy(w => w.StartTime.Date)
            .Select(g => new WorkoutFrequencyData
            {
                Date = g.Key,
                WorkoutCount = g.Count(),
                TotalDuration = TimeSpan.FromTicks(g.Sum(w => w.Duration?.Ticks ?? 0))
            })
            .OrderBy(f => f.Date);

        return frequencyData;
    }

    public async Task<PersonalRecords> GetPersonalRecordsAsync(string userId)
    {
        var workouts = await _workoutService.GetWorkoutsByUserAsync(userId);
        var completedWorkouts = workouts.Where(w => w.Status == WorkoutStatus.Completed);

        var records = new Dictionary<string, PersonalRecord>();

        foreach (var workout in completedWorkouts)
        {
            foreach (var exercise in workout.Exercises)
            {
                var weightSets = exercise.Sets.Where(s => s.Weight.HasValue && s.Reps.HasValue && s.IsCompleted);
                
                foreach (var set in weightSets)
                {
                    var oneRepMax = CalculateOneRepMax(set.Weight!.Value, set.Reps!.Value);
                    
                    if (!records.ContainsKey(exercise.ExerciseId) || records[exercise.ExerciseId].OneRepMax < oneRepMax)
                    {
                        records[exercise.ExerciseId] = new PersonalRecord
                        {
                            ExerciseId = exercise.ExerciseId,
                            ExerciseName = exercise.ExerciseName,
                            Weight = set.Weight!.Value,
                            Reps = set.Reps!.Value,
                            OneRepMax = oneRepMax,
                            AchievedDate = workout.StartTime
                        };
                    }
                }
            }
        }

        return new PersonalRecords
        {
            Records = records.Values.OrderByDescending(r => r.OneRepMax).ToList()
        };
    }

    public async Task<DatNetWorkoutTracker.Workouts.Services.WorkoutStatistics> GetWorkoutStatisticsAsync(string userId, TimeSpan period)
    {
        var fromDate = DateTime.UtcNow.Subtract(period);
        return await _workoutService.GetWorkoutStatisticsAsync(userId, fromDate);
    }

    public async Task<IEnumerable<MuscleGroupData>> GetMuscleGroupDistributionAsync(string userId, TimeSpan period)
    {
        var fromDate = DateTime.UtcNow.Subtract(period);
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, fromDate, DateTime.UtcNow);
        
        var muscleGroupCounts = new Dictionary<string, int>();
        
        foreach (var workout in workouts.Where(w => w.Status == WorkoutStatus.Completed))
        {
            foreach (var exercise in workout.Exercises)
            {
                // This would need to be mapped from exercise data - for now using exercise name as placeholder
                var muscleGroup = exercise.ExerciseName.Contains("Chest") ? "Chest" :
                                exercise.ExerciseName.Contains("Back") ? "Back" :
                                exercise.ExerciseName.Contains("Leg") ? "Legs" :
                                exercise.ExerciseName.Contains("Shoulder") ? "Shoulders" :
                                exercise.ExerciseName.Contains("Arm") ? "Arms" : "Other";
                
                muscleGroupCounts[muscleGroup] = muscleGroupCounts.GetValueOrDefault(muscleGroup, 0) + 1;
            }
        }
        
        return muscleGroupCounts.Select(kvp => new MuscleGroupData
        {
            MuscleGroup = kvp.Key,
            WorkoutCount = kvp.Value,
            TotalVolume = 0, // Could be calculated if needed
            TotalTime = TimeSpan.Zero // Could be calculated if needed
        });
    }

    public async Task<IEnumerable<VolumeProgressData>> GetVolumeProgressAsync(string userId, TimeSpan period)
    {
        var fromDate = DateTime.UtcNow.Subtract(period);
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, fromDate, DateTime.UtcNow);
        
        var weeklyVolume = new List<VolumeProgressData>();
        var groupedByWeek = workouts
            .Where(w => w.Status == WorkoutStatus.Completed)
            .GroupBy(w => GetWeekOfYear(w.StartTime))
            .OrderBy(g => g.Key);
        
        foreach (var weekGroup in groupedByWeek)
        {
            var totalVolume = weekGroup
                .SelectMany(w => w.Exercises)
                .SelectMany(e => e.Sets)
                .Where(s => s.Reps.HasValue && s.Weight.HasValue)
                .Sum(s => s.Reps.Value * s.Weight.Value);
            
            weeklyVolume.Add(new VolumeProgressData
            {
                Date = weekGroup.First().StartTime, // Use the first workout date of the week
                Volume = totalVolume,
                ExerciseName = "All Exercises" // Could be more specific if needed
            });
        }
        
        return weeklyVolume;
    }

    public async Task<IEnumerable<ExerciseFrequencyData>> GetExerciseFrequencyAsync(string userId, TimeSpan period)
    {
        var fromDate = DateTime.UtcNow.Subtract(period);
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, fromDate, DateTime.UtcNow);
        
        var exerciseCounts = new Dictionary<string, int>();
        
        foreach (var workout in workouts.Where(w => w.Status == WorkoutStatus.Completed))
        {
            foreach (var exercise in workout.Exercises)
            {
                exerciseCounts[exercise.ExerciseName] = exerciseCounts.GetValueOrDefault(exercise.ExerciseName, 0) + 1;
            }
        }
        
        return exerciseCounts.Select(kvp => new ExerciseFrequencyData
        {
            ExerciseName = kvp.Key,
            Frequency = kvp.Value,
            AverageWeight = 0, // Could be calculated if needed
            TotalVolume = 0 // Could be calculated if needed
        }).OrderByDescending(e => e.Frequency);
    }

    public async Task<IEnumerable<WeightProgressData>> GetWeightProgressAsync(string userId, string exerciseId, TimeSpan period)
    {
        var fromDate = DateTime.UtcNow.Subtract(period);
        var workouts = await _workoutService.GetWorkoutsByDateRangeAsync(userId, fromDate, DateTime.UtcNow);
        
        var exerciseWorkouts = workouts
            .Where(w => w.Status == WorkoutStatus.Completed && w.Exercises.Any(e => e.ExerciseId == exerciseId))
            .OrderBy(w => w.StartTime);
        
        var progressData = new List<WeightProgressData>();
        
        foreach (var workout in exerciseWorkouts)
        {
            var exercise = workout.Exercises.First(e => e.ExerciseId == exerciseId);
            var maxWeight = exercise.Sets
                .Where(s => s.Weight.HasValue)
                .DefaultIfEmpty()
                .Max(s => s?.Weight ?? 0);
            
            if (maxWeight > 0)
            {
                progressData.Add(new WeightProgressData
                {
                    Date = workout.StartTime,
                    Weight = maxWeight,
                    ExerciseName = exercise.ExerciseName
                });
            }
        }
        
        return progressData;
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var jan1 = new DateTime(date.Year, 1, 1);
        var daysOffset = (int)jan1.DayOfWeek;
        var firstWeekDay = jan1.AddDays(-daysOffset);
        var weekNumber = ((date - firstWeekDay).Days / 7) + 1;
        return weekNumber;
    }

    private static int CalculateOneRepMax(decimal weight, int reps)
    {
        // Using Brzycki formula: 1RM = weight * (36 / (37 - reps))
        if (reps == 1) return (int)weight;
        if (reps > 36) return (int)weight; // Formula breaks down for high reps
        
        return (int)(weight * (36m / (37m - reps)));
    }
}
