// <auto-generated />
using System;
using LCT.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LCT.Infrastructure.Migrations
{
    [DbContext(typeof(LctDbContext))]
    [Migration("20220322080611_player tournament id removal")]
    partial class playertournamentidremoval
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("LCT.Core.Entites.Tournaments.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<Guid?>("TournamentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TournamentId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("LCT.Core.Entites.Tournaments.Entities.SelectedTeam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("TournamentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TournamentId");

                    b.ToTable("SelectedTeams");
                });

            modelBuilder.Entity("LCT.Core.Entites.Tournaments.Entities.Tournament", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Limit")
                        .HasColumnType("int");

                    b.Property<string>("TournamentName")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.HasKey("Id");

                    b.HasIndex("TournamentName")
                        .IsUnique();

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("LCT.Core.Entites.Tournaments.Entities.Player", b =>
                {
                    b.HasOne("LCT.Core.Entites.Tournaments.Entities.Tournament", null)
                        .WithMany("Players")
                        .HasForeignKey("TournamentId");
                });

            modelBuilder.Entity("LCT.Core.Entites.Tournaments.Entities.SelectedTeam", b =>
                {
                    b.HasOne("LCT.Core.Entites.Tournaments.Entities.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");

                    b.HasOne("LCT.Core.Entites.Tournaments.Entities.Tournament", "Tournament")
                        .WithMany("SelectedTeams")
                        .HasForeignKey("TournamentId");

                    b.Navigation("Player");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("LCT.Core.Entites.Tournaments.Entities.Tournament", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("SelectedTeams");
                });
#pragma warning restore 612, 618
        }
    }
}
