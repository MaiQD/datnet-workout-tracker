using MongoDB.Driver;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

public static class ExercisesMongoSeeder
{
    public static void Seed(IMongoDatabase database)
    {
        var muscleGroupCollection = database.GetCollection<MuscleGroup>("muscleGroups");
        var equipmentCollection = database.GetCollection<Equipment>("equipment");

        // Check if data already exists
        if (muscleGroupCollection.CountDocuments(FilterDefinition<MuscleGroup>.Empty) > 0)
        {
            return; // Data already seeded
        }

        // Seed global muscle groups
        var globalMuscleGroups = new List<MuscleGroup>
        {
            // Upper Body
            new()
            {
                Name = "Chest", Description = "Pectoral muscles", BodyRegion = BodyRegion.Upper, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Back", Description = "Back muscles including lats, traps, and rhomboids",
                BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Shoulders", Description = "Deltoid muscles", BodyRegion = BodyRegion.Upper, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Biceps", Description = "Biceps brachii", BodyRegion = BodyRegion.Upper, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Triceps", Description = "Triceps brachii", BodyRegion = BodyRegion.Upper, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Forearms", Description = "Forearm muscles", BodyRegion = BodyRegion.Upper, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },

            // Lower Body
            new()
            {
                Name = "Quadriceps", Description = "Front thigh muscles", BodyRegion = BodyRegion.Lower,
                IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Hamstrings", Description = "Back thigh muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Glutes", Description = "Buttock muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Calves", Description = "Calf muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },

            // Core
            new()
            {
                Name = "Abs", Description = "Abdominal muscles", BodyRegion = BodyRegion.Core, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Obliques", Description = "Side abdominal muscles", BodyRegion = BodyRegion.Core,
                IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Lower Back", Description = "Lower back muscles", BodyRegion = BodyRegion.Core, IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        // Seed global equipment
        var globalEquipment = new List<Equipment>
        {
            new()
            {
                Name = "Barbell", Description = "Standard Olympic barbell", Category = "FreeWeights", IsGlobal = true,
                UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Dumbbells", Description = "Adjustable or fixed weight dumbbells", Category = "FreeWeights",
                IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Pull-up Bar", Description = "Bar for pull-ups and chin-ups", Category = "Bodyweight",
                IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Resistance Bands", Description = "Elastic resistance bands", Category = "Resistance",
                IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Yoga Mat", Description = "Exercise mat for floor work", Category = "Bodyweight",
                IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        // Insert the data
        muscleGroupCollection.InsertMany(globalMuscleGroups);
        equipmentCollection.InsertMany(globalEquipment);
    }
}

