using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineBillingSystem.Models;

public partial class OnlineBillingSystemContext : DbContext
{
    public OnlineBillingSystemContext()
    {
    }

    public OnlineBillingSystemContext(DbContextOptions<OnlineBillingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CustomerDetail> CustomerDetails { get; set; }

    public virtual DbSet<ItemBill> ItemBills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerDetail>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8B96738EB");

            entity.ToTable("CustomerDetail");

            entity.Property(e => e.CardNumber)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ItemBill>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__ItemBill__727E838B1368EBDA");

            entity.ToTable("ItemBill");

            entity.Property(e => e.Discount).HasDefaultValue(0.0);
            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ordered)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Notpaid");
            entity.Property(e => e.Total).HasComputedColumnSql("([Quantity]*[UnitPrice]-[Discount])", false);

            entity.HasOne(d => d.Customer).WithMany(p => p.ItemBills)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ItemBill_Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
