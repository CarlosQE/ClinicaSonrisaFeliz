# 🦷 Clínica Dental Sonrisa Feliz — Backend

API REST en **ASP.NET Core Web API (.NET 10) + Entity Framework Core + SQL Server** para administrar
pacientes, odontólogos, citas, tratamientos y pagos de la Clínica Dental Sonrisa Feliz.
Es solo backend: se prueba con **Swagger**.

El proyecto lo desarrollan 5 grupos en paralelo, un módulo por grupo. Este repositorio es la **base común**:
ya trae el modelo de datos, la base de datos con datos de ejemplo y un módulo terminado (Citas) que sirve de referencia.

> 📘 **Antes de escribir código lee [CONTRIBUTING.md](CONTRIBUTING.md).**

---

## Módulos y responsables

| # | Módulo | Grupo | Rama de trabajo | Estado |
|---|--------|-------|-----------------|--------|
| 1 | Gestión de Pacientes | **ApostolesEvoMorales** | `modulo/pacientes` | ⏳ pendiente |
| 2 | Gestión de Odontólogos | **BagayerosChaco** | `modulo/odontologos` | ⏳ pendiente |
| 3 | Gestión de Citas | **AndresFundamentalistas** (grupo líder) | `modulo/citas` | ✅ terminado (referencia) |
| 4 | Gestión de Tratamientos | **Ultimo** | `modulo/tratamientos` | ⏳ pendiente |
| 5 | Gestión de Pagos | **GuaposMisteriosos** | `modulo/pagos` | ⏳ pendiente |

### Funcionalidades que pide el enunciado

| Módulo | Requisitos |
|--------|------------|
| Pacientes | Registrar pacientes · Actualizar datos personales · Consultar historial de atención · Eliminar registros |
| Odontólogos | Registrar odontólogos · Especialidades · Horarios de trabajo · Estado activo/inactivo |
| Citas | Agendar citas · Reprogramar citas · Cancelar citas · Consultar agenda diaria |
| Tratamientos | Limpieza dental · Ortodoncia · Endodoncia · Extracciones · Registro de observaciones |
| Pagos | Registrar pagos · Consultar deudas pendientes · Emitir comprobantes |

---

## Requisitos

- **.NET SDK 10** → `dotnet --version` debe mostrar `10.x`. Descarga: https://dotnet.microsoft.com/download
- **Visual Studio 2022 (17.14+) o Visual Studio 2026**, con la carga de trabajo *ASP.NET y desarrollo web*.
- **SQL Server LocalDB** (viene con Visual Studio, componente *SQL Server Express LocalDB*). Para comprobarlo: `sqllocaldb info` debe mostrar `MSSQLLocalDB`.
- **Git**.

## Primeros pasos

1. Clona el repositorio y crea la rama de tu módulo (ver tabla de arriba):

   ```bash
   git clone https://github.com/CarlosQE/ClinicaSonrisaFeliz.git
   ```

   ```bash
   cd ClinicaSonrisaFeliz
   ```

   ```bash
   git checkout -b modulo/pacientes
   ```

2. Abre `ClinicaDental.sln` en Visual Studio.

3. Crea la base de datos local. En **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**,
   con *Proyecto predeterminado* = `ClinicaDental.Web`:

   ```powershell
   Update-Database
   ```

   Esto crea la base **`ClinicaDental`** en LocalDB con todas las tablas y los **datos de ejemplo**.
   Sin Visual Studio: `dotnet tool install --global dotnet-ef` y luego
   `dotnet ef database update --project src/ClinicaDental.Web`.

4. Ejecuta el proyecto (**F5**, o por consola):

   ```bash
   dotnet run --project src/ClinicaDental.Web
   ```

   Se abre **Swagger** (`http://localhost:5087/swagger`). Ahí pruebas todos los endpoints.

> Cada vez que hagas `git merge origin/main` y entren cambios de base de datos (carpeta `Migrations/`),
> vuelve a ejecutar `Update-Database`.

### Datos de ejemplo (Guids fijos)

| Qué | Ids |
|-----|-----|
| Especialidades (4) | `10000000-0000-0000-0000-000000000001` … `…0004` |
| Odontólogos (4) | `20000000-0000-0000-0000-000000000001` … `…0004` (el `…0004` está **inactivo**) |
| Pacientes (5) | `30000000-0000-0000-0000-000000000001` … `…0005` |
| Horarios | Lunes a viernes, 08:00–12:00 y 14:00–18:00, para los 4 odontólogos |
| Tratamientos (2) | `50000000-0000-0000-0000-000000000001` (limpieza) y `…0002` (endodoncia) |
| Pago (1) | `60000000-0000-0000-0000-000000000001`, adelanto de 300 Bs al tratamiento `…0002` |

---

## Arquitectura

```
Swagger / cliente HTTP ──► Controller (Controllers/XxxController.cs)
                              │  valida ModelState, llama a las validaciones del servicio
                              │  y responde 200 / 201 / 400 / 404 / 500
                              ▼
                           Servicio (Services/XxxServicio.cs)  implementa  IXxx (Interfaces/)
                              │  consultas y reglas de negocio (métodos bool: ExisteX, ...)
                              ▼
                           BaseDatos (Data/)  ──►  SQL Server LocalDB (ClinicaDental)
```

- **Un solo `DbContext`** compartido: `Data/BaseDatos.cs`, con todas las tablas.
- **Todos los `Id` son `Guid`.** Las claves foráneas se llaman `IdX` (`IdPaciente`, `IdOdontologo`...).
- **`TimeProvider`** en lugar de `DateTime.Now` para obtener la hora actual.
- **Datos de ejemplo** con `HasData` en `Data/DatosIniciales.cs`.

### Estructura del repositorio

```
ClinicaDental.sln
├─ src/ClinicaDental.Web/
│  ├─ Controllers/    Un controlador por módulo                    (cada grupo)
│  ├─ DTOs/<Entidad>/ DTOs de entrada de cada módulo               (cada grupo)
│  ├─ Interfaces/     I<Entidad>.cs de cada módulo                 (cada grupo)
│  ├─ Services/       <Entidad>Servicio.cs de cada módulo          (cada grupo)
│  ├─ Models/         Entidades y enums compartidos                (líder; cada grupo su entidad)
│  ├─ Data/           BaseDatos + DatosIniciales (seed)            (líder)
│  ├─ Migrations/     Migraciones de EF Core                       (líder)
│  ├─ Program.cs      Configuración y registro de servicios        (cada grupo, solo su línea)
│  └─ appsettings.json  Cadena de conexión (LocalDB)               (líder)
├─ tests/             Pruebas automáticas (en reescritura; no forman parte de la solución todavía)
├─ .github/           CI y plantilla de Pull Request
└─ README.md / CONTRIBUTING.md
```

### Modelo de datos

```
Especialidad 1──* Odontologo 1──* HorarioOdontologo
                      │1
Paciente 1──* Cita *──┘
   │1
   └──* Tratamiento *──1 Odontologo      (Tratamiento.IdCita es opcional)
            │1
            └──* Pago
```

| Entidad | Campos clave | Reglas a nivel de base de datos |
|---------|--------------|---------------------------------|
| `Paciente` | Nombres, Apellidos, **CI**, FechaNacimiento, Telefono, Email, Direccion, FechaRegistro | CI único |
| `Especialidad` | **Nombre**, Descripcion | Nombre único |
| `Odontologo` | Nombres, Apellidos, **Matricula**, Telefono, Email, **Activo**, IdEspecialidad | Matrícula única |
| `HorarioOdontologo` | IdOdontologo, DiaSemana, HoraInicio, HoraFin | — |
| `Cita` | IdPaciente, IdOdontologo, FechaHora, DuracionMinutos, Motivo, **Estado**, MotivoCancelacion | — |
| `Tratamiento` | IdPaciente, IdOdontologo, IdCita?, **Tipo**, Fecha, Costo, Observaciones | — |
| `Pago` | IdTratamiento, Monto, FechaPago, Metodo, **NumeroComprobante** | Comprobante único |

Enums (se guardan como número): `EstadoCita` (Programada=0, Reprogramada=1, Cancelada=2, Atendida=3),
`TipoTratamiento` (LimpiezaDental=0, Ortodoncia=1, Endodoncia=2, Extraccion=3),
`MetodoPago` (Efectivo=0, Tarjeta=1, Transferencia=2, QR=3).

**Ninguna clave foránea borra en cascada.** Borrar un registro que otro usa (por ejemplo un paciente con citas)
da error de base de datos: el servicio de cada módulo debe validarlo antes y devolver un `BadRequest` claro.

---

## Módulo 3 — Gestión de Citas (AndresFundamentalistas) ✅

Implementado completo y usado como **ejemplo** para los demás grupos.
Archivos: `DTOs/Cita/`, `Interfaces/ICita.cs`, `Services/CitaServicio.cs`, `Controllers/CitaController.cs`.

| Requisito | Endpoint |
|-----------|----------|
| Listar citas | `GET /api/Cita` |
| Buscar una cita | `GET /api/Cita/{id}` |
| Consultar agenda diaria | `GET /api/Cita/agenda?fecha=yyyy-MM-dd&idOdontologo=&incluirCanceladas=` |
| Agendar cita | `POST /api/Cita` |
| Reprogramar cita | `PUT /api/Cita/{id}/reprogramar` |
| Cancelar cita | `PUT /api/Cita/{id}/cancelar` |

Reglas al agendar/reprogramar: el paciente existe · el odontólogo existe y está **activo** · la fecha/hora es **futura** ·
duración entre 15 y 240 min · la cita cae **completa dentro de una franja** del horario del odontólogo ·
el odontólogo y el paciente no tienen otra cita no cancelada que se **solape** · motivo obligatorio.
Solo se reprograma o cancela una cita **Programada** o **Reprogramada**.
