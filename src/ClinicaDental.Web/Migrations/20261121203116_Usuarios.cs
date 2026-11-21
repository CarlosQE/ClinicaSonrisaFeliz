using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaDental.Web.Migrations
{
    /// <inheritdoc />
    public partial class Usuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: false),
                    BloqueadoHasta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id", "Activo", "BloqueadoHasta", "ContrasenaHash", "FechaRegistro", "IntentosFallidos", "NombreCompleto", "NombreUsuario", "Rol", "UltimoAcceso" },
                values: new object[] { new Guid("70000000-0000-0000-0000-000000000001"), true, null, "PBKDF2$100000$K4v0wtZSoSBERCBGlB7fhQ==$eTwO89QHlPU+KfGKeEJ1AfLBQOmxtKmSps1har/sJ6A=", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Administrador del sistema", "admin", "Administrador", null });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_NombreUsuario",
                table: "usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
