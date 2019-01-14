﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20181216122315_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity("DataLayer.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTimeOffset?>("UpdatedOn");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("DataLayer.AccountStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.ToTable("AccountStatus");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Duy"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Hoang"
                        });
                });

            modelBuilder.Entity("DataLayer.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<long>("UserId");

                    b.Property<int?>("UserId1");

                    b.HasKey("Id");

                    b.HasIndex("UserId1");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("DataLayer.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTimeOffset>("CreatedOn");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTimeOffset?>("UpdatedOn");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("HBD.EfCore.Extensions.Internal.EnumTables<DataLayer.EnumStatus>", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("EnumStatus");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "UnKnow"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Active"
                        },
                        new
                        {
                            Id = 2,
                            Name = "InActive"
                        });
                });

            modelBuilder.Entity("DataLayer.Address", b =>
                {
                    b.HasOne("DataLayer.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId1");

                    b.OwnsOne("DataLayer.OwnedEntity", "OwnedEntity", b1 =>
                        {
                            b1.Property<int>("AddressId");

                            b1.Property<string>("Name");

                            b1.Property<string>("NotReadOnly");

                            b1.Property<string>("ReadOnly");

                            b1.HasKey("AddressId");

                            b1.ToTable("Address");

                            b1.HasOne("DataLayer.Address")
                                .WithOne("OwnedEntity")
                                .HasForeignKey("DataLayer.OwnedEntity", "AddressId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("DataLayer.User", b =>
                {
                    b.HasOne("DataLayer.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");
                });
#pragma warning restore 612, 618
        }
    }
}
