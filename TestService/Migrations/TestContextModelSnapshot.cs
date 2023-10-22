﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestService.Contexts;

#nullable disable

namespace TestService.Migrations
{
    [DbContext(typeof(TestContext))]
    partial class TestContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestService.Models.BasicClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("BasicClassId");

                    b.Property<DateTime>("Oprettet")
                        .HasColumnType("datetime2");

                    b.Property<int>("Refnr")
                        .HasColumnType("int");

                    b.Property<string>("TestField")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("TestService.Models.BasicEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("BasicEntryId");

                    b.Property<Guid>("BasicClassId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ValueToLoad")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BasicClassId");

                    b.ToTable("Entries");
                });

            modelBuilder.Entity("TestService.Models.BasicEntry", b =>
                {
                    b.HasOne("TestService.Models.BasicClass", null)
                        .WithMany("BasicEntries")
                        .HasForeignKey("BasicClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TestService.Models.BasicClass", b =>
                {
                    b.Navigation("BasicEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
