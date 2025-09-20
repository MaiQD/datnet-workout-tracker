using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotFitness.Modules.Users.Infrastructure.Data.Entities;

[Table("OutboxMessages", Schema = "users")]
public class OutboxMessageEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(36)]
    public string EventId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(255)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "jsonb")]
    public string EventData { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsProcessed { get; set; } = false;

    public DateTime? ProcessedAt { get; set; }

    [MaxLength(36)]
    public string? CorrelationId { get; set; }

    [MaxLength(36)]
    public string? TraceId { get; set; }

    public int RetryCount { get; set; } = 0;

    [MaxLength(1000)]
    public string? LastError { get; set; }
}
