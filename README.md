# Clínica Dental Sonrisa Feliz — Backend

API REST en **ASP.NET Core Web API (.NET 10) + Entity Framework Core + SQL Server** para administrar
pacientes, odontólogos, especialidades, horarios, citas, tratamientos y pagos de la Clínica Dental Sonrisa Feliz.
Es solo backend: se prueba con **Swagger**.

Los 5 módulos están implementados: **Pacientes**, **Odontólogos (Especialidades y Horarios)**, **Citas**, **Tratamientos** y **Pagos**.

---

## Requisitos

- **.NET SDK 10** → `dotnet --version` debe mostrar `10.x`. Descarga: https://dotnet.microsoft.com/download
- **Visual Studio 2022 (17.14+) o Visual Studio 2026**, con la carga de trabajo *ASP.NET y desarrollo web*.
- **SQL Server LocalDB** (viene con Visual Studio, componente *SQL Server Express LocalDB*). Para comprobarlo: `sqllocaldb info` debe mostrar `MSSQLLocalDB`.
- **Git**.

## Cómo ejecutar

1. Clona el repositorio y abre `ClinicaDental.sln` en Visual Studio.
2. Crea o actualiza la base de datos local. En **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**, con *Proyecto predeterminado* = `ClinicaDental.Web`:

   ```powershell
   Update-Database
   ```

   Esto crea/actualiza la base **`ClinicaDental`** en LocalDB con las tablas, restricciones y los datos de ejemplo (seed). Sin Visual Studio: `dotnet tool install --global dotnet-ef` y luego `dotnet ef database update --project src/ClinicaDental.Web`.

3. Ejecuta el proyecto:

   ```bash
   dotnet run --project src/ClinicaDental.Web
   ```

   Con el perfil configurado se abre **Swagger** para probar los endpoints (por ejemplo `https://localhost:xxxx/swagger` según `launchSettings`).

> Cada vez que entres cambios de base de datos (carpeta `Migrations/`), vuelve a ejecutar `Update-Database`.

## Módulos y endpoints

Rutas base por controlador: `api/[controller]` (p. ej. `api/Paciente`, `api/Odontologo`, `api/Cita`). Endpoints tomados de los controladores.

### Pacientes (`api/Paciente`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/Paciente` | `ListarPacientes` |
| GET | `/api/Paciente/{id}` | `ObtenerPacientePorId` |
| GET | `/api/Paciente/{id}/historial` | `ConsultarHistorialPaciente` |
| POST | `/api/Paciente` | `RegistrarPaciente` |
| PUT | `/api/Paciente/{id}` | `ActualizarPaciente` |
| DELETE | `/api/Paciente/{id}` | `EliminarPaciente` |

### Odontólogos (`api/Odontologo`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/Odontologo?activo=&idEspecialidad=` | `ListarOdontologos` |
| GET | `/api/Odontologo/{id}` | `ObtenerOdontologoPorId` |
| POST | `/api/Odontologo` | `RegistrarOdontologo` |
| PUT | `/api/Odontologo/{id}` | `ActualizarOdontologo` |
| PUT | `/api/Odontologo/{id}/estado` | `CambiarEstadoOdontologo` |

### Especialidades (`api/Especialidad`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/Especialidad` | `ListarEspecialidades` |
| GET | `/api/Especialidad/{id}` | `ObtenerEspecialidadPorId` |
| POST | `/api/Especialidad` | `RegistrarEspecialidad` |
| PUT | `/api/Especialidad/{id}` | `ActualizarEspecialidad` |
| DELETE | `/api/Especialidad/{id}` | `EliminarEspecialidad` |

### Horarios de odontólogo (`api/HorarioOdontologo`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/HorarioOdontologo/odontologo/{idOdontologo}` | `ListarHorariosOdontologo` |
| GET | `/api/HorarioOdontologo/{id}` | `ObtenerHorarioPorId` |
| POST | `/api/HorarioOdontologo/odontologo/{idOdontologo}` | `RegistrarHorarioOdontologo` |
| PUT | `/api/HorarioOdontologo/{id}` | `ActualizarHorarioOdontologo` |
| DELETE | `/api/HorarioOdontologo/{id}` | `EliminarHorarioOdontologo` |

### Citas (`api/Cita`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/Cita` | `ListarCitas` |
| GET | `/api/Cita/{id}` | `ObtenerCitaPorId` |
| GET | `/api/Cita/agenda?fecha=yyyy-MM-dd&idOdontologo=&incluirCanceladas=` | `ConsultarAgendaDiaria` |
| POST | `/api/Cita` | `AgendarCita` |
| PUT | `/api/Cita/{id}/reprogramar` | `ReprogramarCita` |
| PUT | `/api/Cita/{id}/cancelar` | `CancelarCita` |

### Tratamientos (`api/Tratamiento`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/Tratamiento?idPaciente=&tipo=&idOdontologo=` | `ListarTratamientos` |
| GET | `/api/Tratamiento/{id}` | `ObtenerTratamientoPorId` |
| POST | `/api/Tratamiento` | `RegistrarTratamiento` |
| PUT | `/api/Tratamiento/{id}/observaciones` | `ActualizarObservacionesTratamiento` |
| DELETE | `/api/Tratamiento/{id}` | `EliminarTratamiento` |

### Pagos (`api/Pago`)

| Verbo | Ruta | Name |
|---|---|---|
| GET | `/api/Pago?idPaciente=` | `ListarPagos` |
| GET | `/api/Pago/{id}` | `ObtenerPagoPorId` |
| GET | `/api/Pago/deudas?idPaciente=` | `ConsultarDeudasPendientes` |
| GET | `/api/Pago/tratamiento/{idTratamiento}/saldo` | `ConsultarSaldoTratamiento` |
| GET | `/api/Pago/{id}/comprobante` | `EmitirComprobante` |
| POST | `/api/Pago` | `RegistrarPago` |

## Datos de ejemplo (seed)

El seed se define en `Data/DatosIniciales.cs` y `Factories/*.cs` (aplicado por migraciones/`HasData`). Guids fijos:

| Qué | Ids |
|---|---|
| Especialidades (4) | `10000000-0000-0000-0000-000000000001` … `…0004` |
| Odontólogos (4) | `20000000-0000-0000-0000-000000000001` … `…0004` (el `…0004` está inactivo) |
| Pacientes (5) | `30000000-0000-0000-0000-000000000001` … `…0005` |
| Horarios | Lunes a viernes, 08:00–12:00 y 14:00–18:00, para los 4 odontólogos (`4000…0001`–`…0040`) |
| Tratamientos (2) | `50000000-0000-0000-0000-000000000001` (limpieza) y `…0002` (endodoncia) |
| Pago (1) | `60000000-0000-0000-0000-000000000001`, adelanto de 300 Bs al tratamiento `…0002` |

No se siembran citas de ejemplo.

## Estructura de carpetas (vigente)

```
ClinicaDental.sln
├─ src/ClinicaDental.Web/
│  ├─ Controllers/      Controladores de la API (Cita, Especialidad, HorarioOdontologo, Odontologo, Paciente, Pago, Tratamiento)
│  ├─ DTOs/             DTOs por entidad/módulo (Cita, Especialidad, HorarioOdontologo, Odontologo, Paciente, Pago, Tratamiento)
│  ├─ Data/             BaseDatos.cs (DbContext) y DatosIniciales.cs (índice de seed)
│  ├─ Factories/        Semilla por tabla (Especialidad, Odontologo, HorarioOdontologo, Paciente, Tratamiento, Pago)
│  ├─ Interfaces/       Contratos I<Entidad>.cs
│  ├─ Migrations/       Migraciones de EF Core y snapshot
│  ├─ Models/           Entidades y enums (Paciente, Especialidad, Odontologo, HorarioOdontologo, Cita, Tratamiento, Pago, Enums)
│  ├─ Services/         Implementaciones <Entidad>Servicio.cs
│  ├─ Program.cs         Configuración, DbContext y registro de servicios
│  ├─ appsettings.json  Cadena de conexión (ConexionDB, LocalDB)
│  └─ ClinicaDental.Web.csproj
├─ tests/               Proyecto de pruebas (fuera de la solución; se reescribirá para la API)
├─ .github/             CI y plantilla de Pull Request
├─ README.md
└─ CONTRIBUTING.md
```

## Arquitectura

```
Cliente HTTP / Swagger ──► Controller (Controllers/)
                              │  valida ModelState y reglas expuestas por el servicio
                              │  responde 200/201/204/400/404/500
                              ▼
                           Servicio (Services/<Entidad>Servicio.cs)  implementa  IXxx (Interfaces/)
                              │  consultas y reglas de negocio
                              ▼
                           BaseDatos (Data/BaseDatos.cs) ──► SQL Server LocalDB (ClinicaDental)
```

- **Un solo `DbContext`:** `Data/BaseDatos.cs`.
- **Hora actual:** `TimeProvider` inyectado (no `DateTime.Now`).
- **Seed:** `HasData` configurado desde `Factories/` vía `Data/DatosIniciales.cs`.

## Modelo de datos

```
Especialidad 1──* Odontologo 1──* HorarioOdontologo
                      │1
Paciente 1──* Cita *──┘
   │1
   └──* Tratamiento *──1 Odontologo      (Tratamiento.IdCita es opcional)
            │1
            └──* Pago
```

Enums (guardados como `int`): `EstadoCita` (Programada=0, Reprogramada=1, Cancelada=2, Atendida=3),
`TipoTratamiento` (LimpiezaDental=0, Ortodoncia=1, Endodoncia=2, Extraccion=3),
`MetodoPago` (Efectivo=0, Tarjeta=1, Transferencia=2, QR=3).

## Decisiones de diseño del modelo

- **Ids `Guid` con `[Key]` y semilla con Guids fijos:** garantiza identificadores estables y que el modelo/seed no cambie entre compilaciones.
- **Todas las FK con `DeleteBehavior.Restrict`:** SQL Server rechaza múltiples rutas de cascada y se evitan borrados accidentales de historial clínico; las dependencias se validan antes de eliminar.
- **Enums guardados como `int`:** `EstadoCita`, `TipoTratamiento` y `MetodoPago` se persisten como enteros.
- **Decimales con `decimal(18,2)`:** `Costo` (Tratamiento) y `Monto` (Pago) usan precisión fija para valores monetarios.
- **Índices únicos:** `CI` de Paciente, `Nombre` de Especialidad, `Matrícula` de Odontólogo y `NumeroComprobante` de Pago.
- **Relaciones:** solo 1:N; no hay relaciones 1:1 ni N:M.
- **`Tratamiento.IdCita` opcional:** un tratamiento puede registrarse sin estar asociado a una cita.
- **Seed en `Factories/` referenciado desde `Data/DatosIniciales.cs`:** el seed se organiza por tabla en factories y se invoca desde un único índice.

## Nota sobre pruebas

El proyecto `tests/ClinicaDental.Tests` está **fuera de la solución** y su contenido actual no refleja la API vigente; se reescribirá para la API.
