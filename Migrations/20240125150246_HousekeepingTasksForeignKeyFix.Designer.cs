﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Scheduler.Api.Data;

#nullable disable

namespace Scheduler.Api.Migrations
{
    [DbContext(typeof(ScheduleDb))]
    [Migration("20240125150246_HousekeepingTasksForeignKeyFix")]
    partial class HousekeepingTasksForeignKeyFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Scheduler.Api.Data.Models.Guest", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<int?>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<int?>("Prefix")
                        .HasColumnType("integer");

                    b.Property<int?>("ReservationId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReservationId");

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.User", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<int?>("ScheduleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Housekeepers");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.HousekeepingTask", b =>
                {
                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<int?>("RoomNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("HousekeeperId")
                        .HasColumnType("integer");

                    b.Property<int?>("RoomScheduleId")
                        .HasColumnType("integer");

                    b.Property<int?>("ScheduleId")
                        .HasColumnType("integer");

                    b.Property<int?>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Date", "RoomNumber");

                    b.HasIndex("HousekeeperId");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("RoomNumber", "RoomScheduleId");

                    b.ToTable("HousekeepingTasks");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Reservation", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<int?>("BookingSource")
                        .HasColumnType("integer");

                    b.Property<DateOnly?>("CheckIn")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("CheckOut")
                        .HasColumnType("date");

                    b.Property<string>("FlightArrivalNumber")
                        .HasColumnType("text");

                    b.Property<TimeOnly?>("FlightArrivalTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("FlightDepartureNumber")
                        .HasColumnType("text");

                    b.Property<TimeOnly?>("FlightDepartureTime")
                        .HasColumnType("time without time zone");

                    b.Property<string>("Remarks")
                        .HasColumnType("text");

                    b.Property<int?>("RoomNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("RoomScheduleId")
                        .HasColumnType("integer");

                    b.Property<int?>("RoomType")
                        .HasColumnType("integer");

                    b.Property<int?>("ScheduleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("RoomNumber", "RoomScheduleId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Room", b =>
                {
                    b.Property<int?>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("integer");

                    b.Property<int?>("Floor")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Number", "ScheduleId");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Schedule", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Guest", b =>
                {
                    b.HasOne("Scheduler.Api.Data.Models.Reservation", "Reservation")
                        .WithMany("Guests")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.User", b =>
                {
                    b.HasOne("Scheduler.Api.Data.Models.Schedule", "Schedule")
                        .WithMany("Housekeepers")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.HousekeepingTask", b =>
                {
                    b.HasOne("Scheduler.Api.Data.Models.User", "User")
                        .WithMany("Tasks")
                        .HasForeignKey("HousekeeperId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Scheduler.Api.Data.Models.Schedule", "Schedule")
                        .WithMany("HousekeepingTasks")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Scheduler.Api.Data.Models.Room", "Room")
                        .WithMany("HousekeepingTasks")
                        .HasForeignKey("RoomNumber", "RoomScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");

                    b.Navigation("Room");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Reservation", b =>
                {
                    b.HasOne("Scheduler.Api.Data.Models.Schedule", "Schedule")
                        .WithMany("Reservations")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Scheduler.Api.Data.Models.Room", "Room")
                        .WithMany("Reservations")
                        .HasForeignKey("RoomNumber", "RoomScheduleId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Room");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Room", b =>
                {
                    b.HasOne("Scheduler.Api.Data.Models.Schedule", "Schedule")
                        .WithMany("Rooms")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.User", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Reservation", b =>
                {
                    b.Navigation("Guests");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Room", b =>
                {
                    b.Navigation("HousekeepingTasks");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Scheduler.Api.Data.Models.Schedule", b =>
                {
                    b.Navigation("Housekeepers");

                    b.Navigation("HousekeepingTasks");

                    b.Navigation("Reservations");

                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
