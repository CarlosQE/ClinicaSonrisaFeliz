using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicaDental.Web.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "especialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_especialidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "odontologos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IdEspecialidad = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odontologos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_odontologos_especialidades_IdEspecialidad",
                        column: x => x.IdEspecialidad,
                        principalTable: "especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPaciente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdOdontologo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MotivoCancelacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_citas_odontologos_IdOdontologo",
                        column: x => x.IdOdontologo,
                        principalTable: "odontologos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citas_pacientes_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "horariosOdontologo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdOdontologo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horariosOdontologo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_horariosOdontologo_odontologos_IdOdontologo",
                        column: x => x.IdOdontologo,
                        principalTable: "odontologos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tratamientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPaciente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdOdontologo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCita = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tratamientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tratamientos_citas_IdCita",
                        column: x => x.IdCita,
                        principalTable: "citas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tratamientos_odontologos_IdOdontologo",
                        column: x => x.IdOdontologo,
                        principalTable: "odontologos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tratamientos_pacientes_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTratamiento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metodo = table.Column<int>(type: "int", nullable: false),
                    NumeroComprobante = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagos_tratamientos_IdTratamiento",
                        column: x => x.IdTratamiento,
                        principalTable: "tratamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "especialidades",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Revisión, limpieza y tratamientos básicos", "Odontología General" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Corrección de la posición de los dientes", "Ortodoncia" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Tratamiento de conductos", "Endodoncia" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Extracciones y cirugías", "Cirugía Oral" }
                });

            migrationBuilder.InsertData(
                table: "pacientes",
                columns: new[] { "Id", "Apellidos", "CI", "Direccion", "Email", "FechaNacimiento", "FechaRegistro", "Nombres", "Telefono" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "Quispe Mamani", "7654321", "Av. Blanco Galindo km 4", "maria.quispe@correo.com", new DateOnly(1995, 3, 14), new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "María Fernanda", "71111111" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Rojas Flores", "8123456", "Calle Bolívar #120", null, new DateOnly(1988, 7, 2), new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Juan Carlos", "72222222" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "Valverde Choque", "9012345", null, "lucia.valverde@correo.com", new DateOnly(2001, 11, 25), new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lucía", "73333333" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "Condori Vargas", "6543210", "Zona Sud, calle 5", null, new DateOnly(1979, 1, 9), new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pedro Antonio", "74444444" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "Mendoza Rivero", "10234567", "Av. Circunvalación #45", "sofia.mendoza@correo.com", new DateOnly(2010, 5, 30), new DateTime(2026, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sofía", "75555555" }
                });

            migrationBuilder.InsertData(
                table: "odontologos",
                columns: new[] { "Id", "Activo", "Apellidos", "Email", "IdEspecialidad", "Matricula", "Nombres", "Telefono" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), true, "Salinas Pérez", "ricardo.salinas@sonrisafeliz.bo", new Guid("10000000-0000-0000-0000-000000000001"), "MP-0001", "Ricardo", "70000001" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), true, "Camacho Ortiz", "valeria.camacho@sonrisafeliz.bo", new Guid("10000000-0000-0000-0000-000000000002"), "MP-0002", "Valeria", "70000002" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), true, "Gutiérrez Soria", "marcelo.gutierrez@sonrisafeliz.bo", new Guid("10000000-0000-0000-0000-000000000003"), "MP-0003", "Marcelo", "70000003" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), false, "Lozano Rojas", null, new Guid("10000000-0000-0000-0000-000000000004"), "MP-0004", "Patricia", "70000004" }
                });

            migrationBuilder.InsertData(
                table: "horariosOdontologo",
                columns: new[] { "Id", "DiaSemana", "HoraFin", "HoraInicio", "IdOdontologo" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), 1, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000002"), 1, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000003"), 2, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000004"), 2, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000005"), 3, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000006"), 3, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000007"), 4, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000008"), 4, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000009"), 5, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000010"), 5, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000011"), 1, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000012"), 1, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000013"), 2, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000014"), 2, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000015"), 3, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000016"), 3, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000017"), 4, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000018"), 4, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000019"), 5, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000020"), 5, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000021"), 1, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000022"), 1, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000023"), 2, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000024"), 2, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000025"), 3, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000026"), 3, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000027"), 4, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000028"), 4, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000029"), 5, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000030"), 5, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000031"), 1, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000032"), 1, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000033"), 2, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000034"), 2, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000035"), 3, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000036"), 3, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000037"), 4, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000038"), 4, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000039"), 5, new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("40000000-0000-0000-0000-000000000040"), 5, new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), new Guid("20000000-0000-0000-0000-000000000004") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_citas_IdOdontologo_FechaHora",
                table: "citas",
                columns: new[] { "IdOdontologo", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_citas_IdPaciente_FechaHora",
                table: "citas",
                columns: new[] { "IdPaciente", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_especialidades_Nombre",
                table: "especialidades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_horariosOdontologo_IdOdontologo",
                table: "horariosOdontologo",
                column: "IdOdontologo");

            migrationBuilder.CreateIndex(
                name: "IX_odontologos_IdEspecialidad",
                table: "odontologos",
                column: "IdEspecialidad");

            migrationBuilder.CreateIndex(
                name: "IX_odontologos_Matricula",
                table: "odontologos",
                column: "Matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_CI",
                table: "pacientes",
                column: "CI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_IdTratamiento",
                table: "pagos",
                column: "IdTratamiento");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_NumeroComprobante",
                table: "pagos",
                column: "NumeroComprobante",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tratamientos_IdCita",
                table: "tratamientos",
                column: "IdCita");

            migrationBuilder.CreateIndex(
                name: "IX_tratamientos_IdOdontologo",
                table: "tratamientos",
                column: "IdOdontologo");

            migrationBuilder.CreateIndex(
                name: "IX_tratamientos_IdPaciente",
                table: "tratamientos",
                column: "IdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "horariosOdontologo");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "tratamientos");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "odontologos");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "especialidades");
        }
    }
}
