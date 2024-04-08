using System.Diagnostics.CodeAnalysis;

namespace iRLeagueDatabaseCore.Models;

#nullable enable
public class PaymentEntity
{
    public Guid Id { get; set; }
    public string? PlanId { get; set; }
    public string? SubscriptionId { get; set; }
    public long LeagueId { get; set; }
    public string UserId { get; set; } = default!;
    public PaymentType Type { get; set; }
    public DateTime LastPaymentReceived { get; set; }
    public DateTime? NextPaymentDue { get; set; }
    public PaymentStatus Status { get; set; }

    [MaybeNull]
    public virtual LeagueEntity League { get; set; }
    public virtual SubscriptionEntity? Subscription { get; set; }
}

public enum PaymentType
{
    Subscription,
}

public enum PaymentStatus
{
    Inactive,
    Active,
}

public class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> entity)
    {
        entity.ToTable("Payments");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.PlanId)
            .HasMaxLength(255);

        entity.Property(e => e.SubscriptionId)
            .HasMaxLength(255);

        entity.Property(e => e.UserId)
            .HasMaxLength(255);

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.Type)
            .HasConversion<string>();

        entity.Property(e => e.Status)
            .HasConversion<string>();

        entity.HasOne(d => d.Subscription)
            .WithMany(p => p.Payments)
            .HasForeignKey(d => d.PlanId);
    }
}
