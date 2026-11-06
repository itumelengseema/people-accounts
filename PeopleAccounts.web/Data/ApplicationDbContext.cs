using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PeopleAccounts.web.Models;

namespace PeopleAccounts.web.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasIndex(e => e.AccountNumber, "IX_Account_num").IsUnique();

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("account_number");
            entity.Property(e => e.OutstandingBalance)
                .HasColumnType("money")
                .HasColumnName("outstanding_balance");
            entity.Property(e => e.PersonCode).HasColumnName("person_code");
            entity.Property(e => e.StatusCode)
                .HasDefaultValue(1)
                .HasColumnName("status_code");

            entity.HasOne(d => d.PersonCodeNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.PersonCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Person");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accounts_Status");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasIndex(e => e.IdNumber, "IX_Person_id").IsUnique();

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.IdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("id_number");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("Status");

            entity.HasIndex(e => e.Name, "UQ_Status_Name").IsUnique();

            entity.Property(e => e.Code)
                .ValueGeneratedNever()
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.AccountCode).HasColumnName("account_code");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.CaptureDate)
                .HasColumnType("datetime")
                .HasColumnName("capture_date");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");

            entity.HasOne(d => d.AccountCodeNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Account");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(512)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
