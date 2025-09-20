namespace dotFitness.SharedKernel.Interfaces;

public interface IEntity<TId>
{
    TId Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

// Convenience interface for int IDs (most common case)
public interface IEntity : IEntity<int>
{
}
