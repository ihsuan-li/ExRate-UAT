using System;
using System.Collections.Generic;
using CDFHEXRATE.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace CDFHEXRATE.Repository.Contexts;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExRate> ExRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExRate>(entity =>
        {
           /// entity.HasKey(e => new { e.CountryShortName, e.DataDate, e.ExRateType, e.ReferenceCurrency });
            entity.HasKey(e => new {  e.DataDate, e.ExRateType, e.ReferenceCurrency });

            entity.ToTable("ExRate");

         ///   entity.Property(e => e.CountryShortName)
         ///       .HasMaxLength(10)
         ///       .IsUnicode(false)
         ///       .HasComment("國別");
            entity.Property(e => e.DataDate)
                .HasComment("資料日期")
                .HasColumnType("date");
            entity.Property(e => e.ExRateType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasComment("類型: 1早盤、2收盤");
            entity.Property(e => e.ReferenceCurrency)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("交易國別");
            entity.Property(e => e.LocalBidExRate)
                .HasComment("台幣報價買入")
                .HasColumnType("decimal(20, 8)");
            entity.Property(e => e.LocalOfferExRate)
                .HasComment("台幣報價賣出")
                .HasColumnType("decimal(20, 8)");
            entity.Property(e => e.LocalSettleExRate)
                .HasComment("台幣報價作帳")
                .HasColumnType("decimal(20, 8)");
            entity.Property(e => e.SyncTime)
                .HasComment("排程寫入時間")
                .HasColumnType("datetime");
            entity.Property(e => e.USDBidExRate)
                .HasComment("美元報價買入")
                .HasColumnType("decimal(20, 8)")
                .HasColumnName("USDBidExRate");
            entity.Property(e => e.USDOfferExRate)
                .HasComment("美元報價賣出")
                .HasColumnType("decimal(20, 8)")
                .HasColumnName("USDOfferExRate");
            entity.Property(e => e.USDSettleExRate)
                .HasComment("美元報價作帳")
                .HasColumnType("decimal(20, 8)")
                .HasColumnName("USDSettleExRate");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
