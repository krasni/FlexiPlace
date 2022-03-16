﻿// <auto-generated />
using System;
using FlexiPlace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FlexiPlace.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200819064524_DodanaZahtjevTabela")]
    partial class DodanaZahtjevTabela
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FlexiPlace.Models.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Naziv")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Status");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Naziv = "Za odobravanje"
                        },
                        new
                        {
                            Id = 2,
                            Naziv = "Odobren"
                        },
                        new
                        {
                            Id = 3,
                            Naziv = "Odbijen"
                        },
                        new
                        {
                            Id = 4,
                            Naziv = "Neobrađen"
                        });
                });

            modelBuilder.Entity("FlexiPlace.Models.Zahtjev", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatumOtvaranja");

                    b.Property<string>("Komentar");

                    b.Property<string>("Odobravatelj");

                    b.Property<string>("Podnositelj")
                        .IsRequired();

                    b.Property<string>("RazlogOdbijanja");

                    b.Property<int>("StatusId");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.ToTable("Zahtjev");
                });

            modelBuilder.Entity("FlexiPlace.Models.Zahtjev", b =>
                {
                    b.HasOne("FlexiPlace.Models.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
