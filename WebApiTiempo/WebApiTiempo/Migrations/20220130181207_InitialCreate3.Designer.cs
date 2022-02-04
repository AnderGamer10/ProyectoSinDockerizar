﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiTiempo.Models;

namespace WebApiTiempo.Migrations
{
    [DbContext(typeof(TiempoContext))]
    [Migration("20220130181207_InitialCreate3")]
    partial class InitialCreate3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApiTiempo.Models.InformacionTiempo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GpxX")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GpxY")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Humedad")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Municipio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrecipitacionAcumulada")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Provincia")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Temperatura")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TipoEstacion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VelocidadViento")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TiempoItems");
                });
#pragma warning restore 612, 618
        }
    }
}