// Initialize the database with some sample data
db = db.getSiblingDB('FitnessTrackerDB');

// Create collections and insert sample data
db.exercises.insertMany([
    {
        _id: ObjectId(),
        name: "Push-ups",
        description: "Classic bodyweight exercise targeting chest, shoulders, and triceps",
        muscleGroups: ["Chest", "Shoulders", "Triceps"],
        category: "Strength",
        youtubeLink: "https://www.youtube.com/watch?v=IODxDxX7oi4",
        requiredEquipment: []
    },
    {
        _id: ObjectId(),
        name: "Squats",
        description: "Fundamental lower body exercise targeting legs and glutes",
        muscleGroups: ["Quadriceps", "Glutes", "Hamstrings"],
        category: "Strength",
        youtubeLink: "https://www.youtube.com/watch?v=aclHkVaku9U",
        requiredEquipment: []
    },
    {
        _id: ObjectId(),
        name: "Deadlifts",
        description: "Compound exercise working multiple muscle groups",
        muscleGroups: ["Hamstrings", "Glutes", "Back", "Traps"],
        category: "Strength",
        youtubeLink: "https://www.youtube.com/watch?v=r4MzxtBKyNE",
        requiredEquipment: ["Barbell", "Weight Plates"]
    },
    {
        _id: ObjectId(),
        name: "Running",
        description: "Cardiovascular exercise for endurance and fitness",
        muscleGroups: ["Legs", "Core"],
        category: "Cardio",
        youtubeLink: "https://www.youtube.com/watch?v=brFvyZ4WMoY",
        requiredEquipment: ["Running Shoes"]
    }
]);

db.workoutLogs.insertMany([
    {
        _id: ObjectId(),
        userId: "user123",
        workoutName: "Morning Strength",
        date: new Date("2024-12-15T08:00:00Z"),
        durationMinutes: 45,
        exerciseEntries: [
            {
                exerciseId: db.exercises.findOne({name: "Push-ups"})._id,
                sets: 3,
                repsPerSet: [15, 12, 10],
                weight: null,
                notes: "Feeling strong today"
            },
            {
                exerciseId: db.exercises.findOne({name: "Squats"})._id,
                sets: 3,
                repsPerSet: [20, 18, 15],
                weight: null,
                notes: "Good form maintained"
            }
        ],
        notes: "Great morning workout session"
    }
]);

print("Database initialized with sample data!");
