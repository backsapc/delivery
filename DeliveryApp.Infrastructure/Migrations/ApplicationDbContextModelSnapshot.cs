﻿// <auto-generated />
using System;
using DeliveryApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DeliveryApp.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DeliveryApp.Core.Domain.CourierAggregate.Courier", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("transport_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("transport_id");

                    b.ToTable("couriers", (string)null);
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.CourierAggregate.Transport", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("Speed")
                        .HasColumnType("integer")
                        .HasColumnName("speed");

                    b.HasKey("Id");

                    b.ToTable("transports", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "pedestrian",
                            Speed = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "bicycle",
                            Speed = 2
                        },
                        new
                        {
                            Id = 3,
                            Name = "scooter",
                            Speed = 3
                        },
                        new
                        {
                            Id = 4,
                            Name = "car",
                            Speed = 4
                        });
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.OrderAggregate.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("CourierId")
                        .HasColumnType("uuid")
                        .HasColumnName("courier_id");

                    b.HasKey("Id");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("DeliveryApp.Infrastructure.Entities.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<DateTime>("OccuredOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occuredOnUtc");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("ProcessedOnUtc");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.ToTable("outbox", (string)null);
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.CourierAggregate.Courier", b =>
                {
                    b.HasOne("DeliveryApp.Core.Domain.CourierAggregate.Transport", "Transport")
                        .WithMany()
                        .HasForeignKey("transport_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("DeliveryApp.Core.Domain.SharedKernel.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("CourierId")
                                .HasColumnType("uuid");

                            b1.Property<int>("X")
                                .HasColumnType("integer")
                                .HasColumnName("location_x");

                            b1.Property<int>("Y")
                                .HasColumnType("integer")
                                .HasColumnName("location_y");

                            b1.HasKey("CourierId");

                            b1.ToTable("couriers");

                            b1.WithOwner()
                                .HasForeignKey("CourierId");
                        });

                    b.OwnsOne("DeliveryApp.Core.Domain.CourierAggregate.Status", "Status", b1 =>
                        {
                            b1.Property<Guid>("CourierId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("status");

                            b1.HasKey("CourierId");

                            b1.ToTable("couriers");

                            b1.WithOwner()
                                .HasForeignKey("CourierId");
                        });

                    b.Navigation("Location");

                    b.Navigation("Status");

                    b.Navigation("Transport");
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.CourierAggregate.Transport", b =>
                {
                    b.OwnsOne("DeliveryApp.Core.Domain.SharedKernel.Weight", "Capacity", b1 =>
                        {
                            b1.Property<int>("TransportId")
                                .HasColumnType("integer");

                            b1.Property<int>("Value")
                                .HasColumnType("integer")
                                .HasColumnName("capacity");

                            b1.HasKey("TransportId");

                            b1.ToTable("transports");

                            b1.WithOwner()
                                .HasForeignKey("TransportId");
                        });

                    b.Navigation("Capacity");
                });

            modelBuilder.Entity("DeliveryApp.Core.Domain.OrderAggregate.Order", b =>
                {
                    b.OwnsOne("DeliveryApp.Core.Domain.SharedKernel.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uuid");

                            b1.Property<int>("X")
                                .HasColumnType("integer")
                                .HasColumnName("location_x");

                            b1.Property<int>("Y")
                                .HasColumnType("integer")
                                .HasColumnName("location_y");

                            b1.HasKey("OrderId");

                            b1.ToTable("orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.OwnsOne("DeliveryApp.Core.Domain.SharedKernel.Weight", "Weight", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Value")
                                .HasColumnType("integer")
                                .HasColumnName("weight");

                            b1.HasKey("OrderId");

                            b1.ToTable("orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.OwnsOne("DeliveryApp.Core.Domain.OrderAggregate.Status", "Status", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("status");

                            b1.HasKey("OrderId");

                            b1.ToTable("orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("Location");

                    b.Navigation("Status");

                    b.Navigation("Weight");
                });
#pragma warning restore 612, 618
        }
    }
}
