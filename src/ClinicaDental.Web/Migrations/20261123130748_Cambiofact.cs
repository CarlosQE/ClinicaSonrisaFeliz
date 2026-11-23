using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicaDental.Web.Migrations
{
    /// <inheritdoc />
    public partial class Cambiofact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "citas",
                columns: new[] { "Id", "DuracionMinutos", "Estado", "Fecha", "FechaRegistro", "Hora", "IdOdontologo", "IdPaciente", "Motivo", "MotivoCancelacion" },
                values: new object[,]
                {
                    { new Guid("80000000-0000-0000-0000-000000000001"), 30, 3, new DateOnly(2026, 11, 5), new DateTime(2026, 11, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(9, 0, 0), new Guid("20000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), "Limpieza dental", null },
                    { new Guid("80000000-0000-0000-0000-000000000002"), 60, 3, new DateOnly(2026, 11, 6), new DateTime(2026, 11, 4, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(10, 0, 0), new Guid("20000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000002"), "Dolor de muela, posible conducto", null },
                    { new Guid("80000000-0000-0000-0000-000000000003"), 30, 2, new DateOnly(2026, 11, 10), new DateTime(2026, 11, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000004"), "Control de endodoncia", "El paciente no pudo asistir" },
                    { new Guid("80000000-0000-0000-0000-000000000004"), 30, 0, new DateOnly(2026, 12, 1), new DateTime(2026, 11, 20, 15, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(10, 0, 0), new Guid("20000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000003"), "Control de ortodoncia", null },
                    { new Guid("80000000-0000-0000-0000-000000000005"), 30, 0, new DateOnly(2026, 12, 2), new DateTime(2026, 11, 20, 16, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(15, 0, 0), new Guid("20000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000004"), "Revisión general", null },
                    { new Guid("80000000-0000-0000-0000-000000000006"), 30, 0, new DateOnly(2026, 12, 3), new DateTime(2026, 11, 21, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(9, 30, 0), new Guid("20000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000005"), "Primera consulta de ortodoncia", null },
                    { new Guid("80000000-0000-0000-0000-000000000007"), 30, 1, new DateOnly(2026, 12, 4), new DateTime(2026, 11, 21, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(16, 0, 0), new Guid("20000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000003"), "Limpieza dental", null }
                });

            migrationBuilder.UpdateData(
                table: "tratamientos",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "IdCita",
                value: new Guid("80000000-0000-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "tratamientos",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "IdCita",
                value: new Guid("80000000-0000-0000-0000-000000000002"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "citas",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000007"));

            migrationBuilder.UpdateData(
                table: "tratamientos",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "IdCita",
                value: null);

            migrationBuilder.UpdateData(
                table: "tratamientos",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "IdCita",
                value: null);
        }
    }
}
