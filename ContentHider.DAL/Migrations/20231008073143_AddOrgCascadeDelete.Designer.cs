﻿// <auto-generated />
using ContentHider.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ContentHider.DAL.Migrations
{
    [DbContext(typeof(HiderDbContext))]
    [Migration("20231008073143_AddOrgCascadeDelete")]
    partial class AddOrgCascadeDelete
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ContentHider.Core.Daos.FormatDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("OrganizationId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Formats");
                });

            modelBuilder.Entity("ContentHider.Core.Daos.OrganizationDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("OwnerId")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("ContentHider.Core.Entities.UserDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = "F3994CE5-4E71-4D76-AE9B-923667D1D2C9",
                            FirstName = "AdminFirstName",
                            LastName = "AdminLastName",
                            Password = "admin",
                            Role = 1,
                            UserName = "admin"
                        },
                        new
                        {
                            Id = "24E101BB-6F1B-45C9-ABED-3AC3ED0399DF",
                            FirstName = "UserFirstName",
                            LastName = "UserLastName",
                            Password = "user",
                            Role = 0,
                            UserName = "user"
                        });
                });

            modelBuilder.Entity("ContentHider.Core.Daos.FormatDao", b =>
                {
                    b.HasOne("ContentHider.Core.Daos.OrganizationDao", "Organization")
                        .WithMany("Formats")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("ContentHider.Core.Daos.OrganizationDao", b =>
                {
                    b.Navigation("Formats");
                });
#pragma warning restore 612, 618
        }
    }
}
