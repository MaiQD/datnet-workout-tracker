using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace dotFitness.Modules.Exercises.Application.Mappers;

[Mapper]
public static partial class ExerciseMapper
{
    public static partial ExerciseDto ToDto(Exercise exercise);
    public static partial IEnumerable<ExerciseDto> ToDto(IEnumerable<Exercise> exercises);
}

[Mapper]
public static partial class MuscleGroupMapper
{
    public static partial MuscleGroupDto ToDto(MuscleGroup muscleGroup);
    public static partial IEnumerable<MuscleGroupDto> ToDto(IEnumerable<MuscleGroup> muscleGroups);
}

[Mapper]
public static partial class EquipmentMapper
{
    public static partial EquipmentDto ToDto(Equipment equipment);
    public static partial IEnumerable<EquipmentDto> ToDto(IEnumerable<Equipment> equipment);
}
