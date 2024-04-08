using System.Diagnostics.CodeAnalysis;

namespace iRLeagueDatabaseCore.Models;

#nullable enable
public class SubscriptionEntity
{
    public string PlanId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public SubscriptionInterval Interval { get; set; }
    public double Price { get; set; }

    [MaybeNull]
    public virtual IEnumerable<PaymentEntity> Payments { get; set; }
}

public enum SubscriptionInterval
{
    Monthly,
    Yearly,
}

public class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> entity)
    {
        entity.ToTable("Subscriptions");

        entity.HasKey(e => e.PlanId);

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();
    }
}
