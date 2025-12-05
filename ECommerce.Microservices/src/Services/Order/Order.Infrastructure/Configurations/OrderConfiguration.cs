using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CreatedBy)
            .HasMaxLength(100);

        builder.Property(o => o.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Owned entity for ShippingAddress
        builder.OwnsOne(o => o.ShippingAddress, sa =>
        {
            sa.Property(a => a.Street).HasMaxLength(200).IsRequired();
            sa.Property(a => a.City).HasMaxLength(100).IsRequired();
            sa.Property(a => a.State).HasMaxLength(100);
            sa.Property(a => a.Country).HasMaxLength(100).IsRequired();
            sa.Property(a => a.ZipCode).HasMaxLength(20);
        });

        // Owned entity for BillingAddress
        builder.OwnsOne(o => o.BillingAddress, ba =>
        {
            ba.Property(a => a.Street).HasMaxLength(200).IsRequired();
            ba.Property(a => a.City).HasMaxLength(100).IsRequired();
            ba.Property(a => a.State).HasMaxLength(100);
            ba.Property(a => a.Country).HasMaxLength(100).IsRequired();
            ba.Property(a => a.ZipCode).HasMaxLength(20);
        });

        // OrderStatus enum conversion
        builder.Property(o => o.Status)
            .HasConversion(
                s => s.Id,
                id => Domain.Entities.OrderStatus.GetAll<Domain.Entities.OrderStatus>()
                    .First(s => s.Id == id))
            .IsRequired();

        // Relationship with OrderItems (using shadow property)
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter for soft delete
        builder.HasQueryFilter(o => !o.IsDeleted);

        // Ignore domain events
        builder.Ignore(o => o.DomainEvents);
    }
}
