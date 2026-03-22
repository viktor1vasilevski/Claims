namespace Claims.Infrastructure.Configurations;

public class CoverConfiguration : IEntityTypeConfiguration<Cover>
{
    public void Configure(EntityTypeBuilder<Cover> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Premium)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
