using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using dotFitness.Modules.Exercises.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Exercises.Infrastructure.HealthChecks;

/// <summary>
/// Health check for the Exercises module
/// </summary>
public class ExercisesModuleHealthCheck : IHealthCheck
{
    private readonly IMongoCollection<Exercise> _exercisesCollection;
    private readonly IMongoCollection<MuscleGroup> _muscleGroupsCollection;
    private readonly IMongoCollection<Equipment> _equipmentCollection;
    private readonly ILogger<ExercisesModuleHealthCheck> _logger;

    public ExercisesModuleHealthCheck(
        IMongoCollection<Exercise> exercisesCollection,
        IMongoCollection<MuscleGroup> muscleGroupsCollection,
        IMongoCollection<Equipment> equipmentCollection,
        ILogger<ExercisesModuleHealthCheck> logger)
    {
        _exercisesCollection = exercisesCollection;
        _muscleGroupsCollection = muscleGroupsCollection;
        _equipmentCollection = equipmentCollection;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            ["module"] = "Exercises",
            ["database"] = "MongoDB"
        };

        try
        {
            // Test MongoDB connections and get counts
            var exerciseCount = await _exercisesCollection.CountDocumentsAsync(
                FilterDefinition<Exercise>.Empty, 
                cancellationToken: cancellationToken);

            var muscleGroupCount = await _muscleGroupsCollection.CountDocumentsAsync(
                FilterDefinition<MuscleGroup>.Empty,
                cancellationToken: cancellationToken);

            var equipmentCount = await _equipmentCollection.CountDocumentsAsync(
                FilterDefinition<Equipment>.Empty,
                cancellationToken: cancellationToken);

            data["mongoConnection"] = "OK";
            data["exerciseCount"] = exerciseCount;
            data["muscleGroupCount"] = muscleGroupCount;
            data["equipmentCount"] = equipmentCount;

            _logger.LogDebug("Exercises module health check passed. Exercises: {ExerciseCount}, MuscleGroups: {MuscleGroupCount}, Equipment: {EquipmentCount}", 
                exerciseCount, muscleGroupCount, equipmentCount);

            return HealthCheckResult.Healthy(
                $"Exercises module healthy. Exercises: {exerciseCount}, MuscleGroups: {muscleGroupCount}, Equipment: {equipmentCount}", 
                data);
        }
        catch (Exception ex)
        {
            data["error"] = ex.Message;
            _logger.LogError(ex, "Exercises module health check failed");
            return HealthCheckResult.Unhealthy($"Exercises module unhealthy: {ex.Message}", ex, data);
        }
    }
}
