//namespace iRLeagueDatabaseCore.Models;

//public partial class FilterConditionEntity
//{
//    public FilterConditionEntity()
//    {
//        FilterValues = new HashSet<string>();
//    }

//    public long LeagueId { get; set; }
//    public long ConditionId { get; set; }
//    public long FilterOptionId { get; set; }

//    public FilterType FilterType { get; set; }
//    public string ColumnPropertyName { get; set; }
//    public ComparatorType Comparator { get; set; }
//    public MatchedValueAction Action { get; set; }

//    public virtual FilterOptionEntity FilterOption { get; set; }
//    public virtual ICollection<string> FilterValues { get; set; }
//}

//public sealed class FilterConditionEntityConfiguration : IEntityTypeConfiguration<FilterConditionEntity>
//{
//    public void Configure(EntityTypeBuilder<FilterConditionEntity> entity)
//    {
//        entity.HasKey(e => new { e.LeagueId, e.ConditionId });

//        entity.HasAlternateKey(e => e.ConditionId);

//        entity.Property(e => e.ConditionId)
//            .ValueGeneratedOnAdd();

//        entity.Property(e => e.FilterValues)
//            .HasConversion(new CollectionToStringConverter<string>(), new ValueComparer<ICollection<string>>(true));

//        entity.Property(e => e.FilterType).HasConversion<EnumToStringConverter<FilterType>>();

//        entity.Property(e => e.Comparator).HasConversion<EnumToStringConverter<ComparatorType>>();

//        entity.Property(e => e.Action).HasConversion<EnumToStringConverter<MatchedValueAction>>();

//        entity.HasOne(d => d.FilterOption)
//            .WithMany(p => p.Conditions)
//            .HasForeignKey(d => new { d.LeagueId, d.FilterOptionId });
//    }
//}
