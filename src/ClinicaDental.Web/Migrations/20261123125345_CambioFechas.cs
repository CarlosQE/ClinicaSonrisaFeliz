using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaDental.Web.Migrations
{
    /// <inheritdoc />
    public partial class CambioFechas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_citas_IdOdontologo_FechaHora",
                table: "citas");

            migrationBuilder.DropIndex(
                name: "IX_citas_IdPaciente_FechaHora",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "FechaHora",
                table: "citas");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Fecha",
                table: "citas",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Hora",
                table: "citas",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_citas_IdOdontologo_Fecha_Hora",
                table: "citas",
                columns: new[] { "IdOdontologo", "Fecha", "Hora" });

            migrationBuilder.CreateIndex(
                name: "IX_citas_IdPaciente_Fecha_Hora",
                table: "citas",
                columns: new[] { "IdPaciente", "Fecha", "Hora" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_citas_IdOdontologo_Fecha_Hora",
                table: "citas");

            migrationBuilder.DropIndex(
                name: "IX_citas_IdPaciente_Fecha_Hora",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "Hora",
                table: "citas");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHora",
                table: "citas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_citas_IdOdontologo_FechaHora",
                table: "citas",
                columns: new[] { "IdOdontologo", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_citas_IdPaciente_FechaHora",
                table: "citas",
                columns: new[] { "IdPaciente", "FechaHora" });
        }
    }
}
