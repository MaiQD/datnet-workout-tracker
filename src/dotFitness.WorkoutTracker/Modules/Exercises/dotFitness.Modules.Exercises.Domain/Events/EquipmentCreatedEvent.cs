namespace dotFitness.Modules.Exercises.Domain.Events;

public class EquipmentCreatedEvent
{
    public string EquipmentId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EquipmentCreatedEvent() { }

    public EquipmentCreatedEvent(string equipmentId, string? userId, string name, bool isGlobal)
    {
        EquipmentId = equipmentId;
        UserId = userId;
        Name = name;
        IsGlobal = isGlobal;
        CreatedAt = DateTime.UtcNow;
    }
}
