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
    [Migration("20200926143637_Mijenjamstoruopet")]
    partial class Mijenjamstoruopet
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FlexiPlace.Models.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LoginName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Admin");
                });

            modelBuilder.Entity("FlexiPlace.Models.Ispravnost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IspravnostStatus");

                    b.HasKey("Id");

                    b.ToTable("Ispravnost");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IspravnostStatus = "Zadovoljava uvjete"
                        },
                        new
                        {
                            Id = 2,
                            IspravnostStatus = "Ne zadovljava uvjete"
                        });
                });

            modelBuilder.Entity("FlexiPlace.Models.NeradniDanHanfa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("NeradniDan");

                    b.HasKey("Id");

                    b.ToTable("NeradniDanHanfa");
                });

            modelBuilder.Entity("FlexiPlace.Models.Parametar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DozvoljeniBrojDanaMjesecu");

                    b.Property<int>("DozvoljeniBrojaDanaOdobrenje");

                    b.HasKey("Id");

                    b.ToTable("Parametar");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DozvoljeniBrojDanaMjesecu = 10,
                            DozvoljeniBrojaDanaOdobrenje = 10
                        });
                });

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

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DatumOdsustva");

                    b.Property<DateTime>("DatumOtvaranja");

                    b.Property<string>("ImePrezimePodnositelj");

                    b.Property<int>("IspravnostId");

                    b.Property<string>("Komentar");

                    b.Property<string>("ModifedBy");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("OdobravateljADName");

                    b.Property<string>("OdobravateljEmail");

                    b.Property<string>("OdobravateljImePrezime");

                    b.Property<string>("OdobravateljNazivOrganizacijskeJedinice");

                    b.Property<string>("OdobravateljPutanja");

                    b.Property<string>("OrganizacijskaJedinicaPodnositelj");

                    b.Property<string>("OrganizacijskaJedinicaPutanjaPodnositelj");

                    b.Property<string>("Podnositelj")
                        .IsRequired();

                    b.Property<string>("PodnositeljEmail");

                    b.Property<string>("RazlogOdbijanja");

                    b.Property<int>("StatusId");

                    b.HasKey("Id");

                    b.HasIndex("IspravnostId");

                    b.HasIndex("StatusId");

                    b.ToTable("Zahtjev");
                });

            modelBuilder.Entity("FlexiPlace.Models.Zahtjev", b =>
                {
                    b.HasOne("FlexiPlace.Models.Ispravnost", "Ispravnost")
                        .WithMany()
                        .HasForeignKey("IspravnostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("FlexiPlace.Models.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}