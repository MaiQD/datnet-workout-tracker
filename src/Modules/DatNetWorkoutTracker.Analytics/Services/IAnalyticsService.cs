using DatNetWorkoutTracker.Workouts.Services;

namespace DatNetWorkoutTracker.Analytics.Services;

public interface IAnalyticsService
{
    Task<ProgressReport> GetProgressReportAsync(string userId, DateTime fromDate, DateTime toDate);
    Task<StrengthProgressData> GetStrengthProgressAsync(string userId, string exerciseId, DateTime fromDate, DateTime toDate);
    Task<IEnumerable<WorkoutFrequencyData>> GetWorkoutFrequencyAsync(string userId, TimeSpan period);
    Task<PersonalRecords> GetPersonalRecordsAsync(string userId);
    
    // Additional methods needed by Analytics.razor
    Task<DatNetWorkoutTracker.Workouts.Services.WorkoutStatistics> GetWorkoutStatisticsAsync(string userId, TimeSpan period);
    Task<IEnumerable<MuscleGroupData>> GetMuscleGroupDistributionAsync(string userId, TimeSpan period);
    Task<IEnumerable<VolumeProgressData>> GetVolumeProgressAsync(string userId, TimeSpan period);
    Task<IEnumerable<ExerciseFrequencyData>> GetExerciseFrequencyAsync(string userId, TimeSpan period);
    Task<IEnumerable<WeightProgressData>> GetWeightProgressAsync(string userId, string exerciseId, TimeSpan period);
}

public class ProgressReport
{
    public DatNetWorkoutTracker.Workouts.Services.WorkoutStatistics Statistics { get; set; } = new();
    public List<StrengthProgressData> StrengthProgress { get; set; } = new();
    public List<WorkoutFrequencyData> WorkoutFrequency { get; set; } = new();
    public PersonalRecords PersonalRecords { get; set; } = new();
}

public class StrengthProgressData
{
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public List<StrengthDataPoint> DataPoints { get; set; } = new();
}

public class StrengthDataPoint
{
    public DateTime Date { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal TotalVolume { get; set; }
    public int OneRepMax { get; set; }
}

public class WorkoutFrequencyData
{
    public DateTime Date { get; set; }
    public int WorkoutCount { get; set; }
    public TimeSpan TotalDuration { get; set; }
}

public class PersonalRecords
{
    public List<PersonalRecord> Records { get; set; } = new();
}

public class PersonalRecord
{
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int Reps { get; set; }
    public int OneRepMax { get; set; }
    public DateTime AchievedDate { get; set; }
}

public class MuscleGroupData
{
    public string MuscleGroup { get; set; } = string.Empty;
    public int WorkoutCount { get; set; }
    public decimal TotalVolume { get; set; }
    public TimeSpan TotalTime { get; set; }
}

public class WeightProgressData
{
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
}

public class VolumeProgressData
{
    public DateTime Date { get; set; }
    public decimal Volume { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
}

public class ExerciseFrequencyData
{
    public string ExerciseName { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public decimal AverageWeight { get; set; }
    public decimal TotalVolume { get; set; }
}
