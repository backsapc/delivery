using DeliveryApp.Core.Domain.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.EntityConfigurations.CourierAggregate
{
    class CourierEntityTypeConfiguration : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("couriers");

            entityTypeBuilder.HasKey(entity => entity.Id);

            entityTypeBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.Name)
                .HasColumnName("name")
                .IsRequired();

            entityTypeBuilder
                .HasOne(entity => entity.Transport)
                .WithMany()
                .IsRequired()
                .HasForeignKey("transport_id");
                
            entityTypeBuilder
                .Property(entity => entity.CurrentOrderId)
                .HasColumnName("current_order_id");

            entityTypeBuilder
                .OwnsOne(entity => entity.CourierLocation, l =>
                {
                    l.Property(x => x.PositionX).HasColumnName("location_x").IsRequired();
                    l.Property(y => y.PositionY).HasColumnName("location_y").IsRequired();
                    l.WithOwner();
                });

            entityTypeBuilder
                .OwnsOne(entity => entity.Status, a =>
                {
                    a.Property(c => c.Value).HasColumnName("status").IsRequired();
                    a.WithOwner();
                });
        }
    }
}