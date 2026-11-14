using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicaDental.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedTratamientosPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tratamientos",
                columns: new[] { "Id", "Costo", "Fecha", "IdCita", "IdOdontologo", "IdPaciente", "Observaciones", "Tipo" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), 150.00m, new DateOnly(2026, 11, 5), null, new Guid("20000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), "Limpieza y profilaxis completa", 0 },
                    { new Guid("50000000-0000-0000-0000-000000000002"), 800.00m, new DateOnly(2026, 11, 6), null, new Guid("20000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000002"), "Tratamiento de conducto, primera sesión", 2 }
                });

            migrationBuilder.InsertData(
                table: "pagos",
                columns: new[] { "Id", "FechaPago", "IdTratamiento", "Metodo", "Monto", "NumeroComprobante", "Observacion" },
                values: new object[] { new Guid("60000000-0000-0000-0000-000000000001"), new DateTime(2026, 11, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), new Guid("50000000-0000-0000-0000-000000000002"), 0, 300.00m, "CMP-000001", "Adelanto" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "pagos",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "tratamientos",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "tratamientos",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));
        }
    }
}
