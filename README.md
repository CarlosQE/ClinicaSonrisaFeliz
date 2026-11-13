# 🦷 Clínica Dental Sonrisa Feliz — Sistema de Gestión

Sistema web en **ASP.NET Core MVC (.NET 10) + Entity Framework Core + SQLite** para administrar
pacientes, odontólogos, citas, tratamientos y pagos de la Clínica Dental Sonrisa Feliz.

El proyecto lo desarrollan 5 grupos en paralelo. Este repositorio es la **base común**: ya trae el modelo
de datos, los contratos (interfaces) de cada módulo, el menú, los datos de ejemplo y **las pruebas
automáticas que cada grupo debe hacer pasar**. Si todas las pruebas están en verde, los módulos encajan
entre sí y el programa final funciona.

> 📘 **Antes de escribir código lee [CONTRIBUTING.md](CONTRIBUTING.md)** (guía paso a paso para los grupos).

---

## Módulos y responsables

| # | Módulo | Grupo | Servicio a implementar | Pruebas |
|---|--------|-------|------------------------|---------|
| 1 | Gestión de Pacientes | **ApostolesEvoMorales** | `Services/PacienteService.cs` | `dotnet test --filter "Modulo=Pacientes"` |
| 2 | Gestión de Odontólogos | **BagayerosChaco** | `Services/OdontologoService.cs` | `dotnet test --filter "Modulo=Odontologos"` |
| 3 | Gestión de Citas | **AndresFundamentalistas** (líder) | `Services/CitaService.cs` ✅ terminado | `dotnet test --filter "Modulo=Citas"` |
| 4 | Gestión de Tratamientos | **Ultimo** | `Services/TratamientoService.cs` | `dotnet test --filter "Modulo=Tratamientos"` |
| 5 | Gestión de Pagos | **GuaposMisteriosos** | `Services/PagoService.cs` | `dotnet test --filter "Modulo=Pagos"` |
| — | Infraestructura común | Líder | (modelo, contexto, layout) | `dotnet test --filter "Modulo=Base"` |

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

- **.NET SDK 10** → `dotnet --version` debe mostrar `10.x` o superior. Descarga: https://dotnet.microsoft.com/download
- Un editor: **Visual Studio 2022 (17.14+) / Visual Studio 2026**, o **VS Code** con la extensión *C# Dev Kit*, o **Rider**.
- **Git**.
- No hace falta instalar ningún motor de base de datos: se usa **SQLite** (un archivo `clinica.db`).

## Primeros pasos

```bash
git clone <URL-DEL-REPOSITORIO>
```

```bash
cd ProyectoProgramacionWeb2
```

```bash
dotnet build
```

```bash
dotnet run --project src/ClinicaDental.Web
```

Abre la URL que aparece en consola (por ejemplo `http://localhost:5xxx`). La primera vez se crea
`clinica.db` con **datos de ejemplo** (4 odontólogos, 5 pacientes, citas para los próximos días hábiles,
un tratamiento y un pago). Los módulos que aún no están implementados muestran "🚧 Módulo en construcción".

Para correr **todas** las pruebas:

```bash
dotnet test
```

Para correr solo las de un módulo (lo que normalmente harás):

```bash
dotnet test --filter "Modulo=Pacientes"
```

En Visual Studio: *Prueba → Explorador de pruebas →* agrupar por **Rasgos (Traits)** para ver cada módulo por separado.

### Estado actual de las pruebas

| Categoría | Estado esperado hoy |
|-----------|--------------------|
| `Modulo=Base` | ✅ verde (siempre debe estarlo) |
| `Modulo=Citas` | ✅ verde (módulo terminado, sirve de ejemplo) |
| `Modulo=Pacientes` / `Odontologos` / `Tratamientos` / `Pagos` | ❌ rojo hasta que cada grupo implemente su módulo |

Es normal que `dotnet test` (todo junto) salga en rojo mientras haya módulos pendientes. Tu objetivo es
dejar **tu** módulo en verde sin romper `Base` ni los demás.

---

## Arquitectura

```
Navegador ──► Controller (Controllers/XxxController.cs)
                 │   solo coordina: valida ModelState, llama al servicio, elige la vista
                 ▼
              Servicio (Services/XxxService.cs)  implementa  IXxxService (Services/Contratos/)
                 │   TODAS las reglas de negocio viven aquí
                 ▼
              ClinicaDbContext (Data/)  ──►  SQLite (clinica.db)
```

- **Un solo `DbContext`** compartido (`Data/ClinicaDbContext.cs`) con todas las tablas.
- **Contratos congelados**: cada módulo tiene una interfaz en `Services/Contratos/`. Los otros módulos y
  las pruebas dependen de esas firmas, por eso no se cambian sin acuerdo (un test lo verifica).
- **Dos excepciones estándar** (`Common/Excepciones.cs`):
  `ReglaNegocioException` (regla violada → se muestra en el formulario) y
  `NoEncontradoException` (id inexistente → 404 automático por el filtro global).
- **Inyección de dependencias ya configurada** en `Common/RegistroServicios.cs`: los grupos no tocan `Program.cs`.
- **`TimeProvider`** en lugar de `DateTime.Now`, para que las pruebas controlen "la hora actual".

### Estructura del repositorio

```
ClinicaDental.sln
├─ src/ClinicaDental.Web/
│  ├─ Common/              Excepciones, registro de servicios, filtro global        (líder)
│  ├─ Data/                ClinicaDbContext + DatosIniciales (datos de ejemplo)      (líder)
│  ├─ Models/              Entidades compartidas + Models/Consultas (DTOs)            (contrato)
│  ├─ Services/Contratos/  Interfaces de los 5 módulos                                (contrato)
│  ├─ Services/            Implementación de cada módulo                              (cada grupo)
│  ├─ Controllers/         Un controlador por módulo                                  (cada grupo)
│  ├─ ViewModels/          Modelos de formularios de cada módulo                      (cada grupo)
│  └─ Views/<Modulo>/      Vistas de cada módulo                                      (cada grupo)
├─ tests/ClinicaDental.Tests/
│  ├─ Infraestructura/     BD en memoria, reloj fijo, Escenario (datos), WebFactory   (líder)
│  ├─ Base/                Pruebas del modelo y de los contratos                      (líder)
│  ├─ Modulos/<Modulo>/    Pruebas unitarias del servicio de cada módulo
│  └─ Integracion/         Levanta la app completa y prueba páginas y formularios
├─ .github/workflows/ci.yml   Corre las pruebas de cada módulo en cada Pull Request
├─ README.md / CONTRIBUTING.md
```

### Modelo de datos

```
Especialidad 1──* Odontologo 1──* HorarioOdontologo
                      │1
Paciente 1──* Cita *──┘
   │1          │1
   └──* Tratamiento *──1 Odontologo      (Tratamiento.CitaId es opcional)
            │1
            └──* Pago
```

| Entidad | Campos clave | Reglas a nivel de base de datos |
|---------|--------------|---------------------------------|
| `Paciente` | Nombres, Apellidos, **CI**, FechaNacimiento, Telefono, Email, Direccion, FechaRegistro | CI único |
| `Especialidad` | **Nombre**, Descripcion | Nombre único |
| `Odontologo` | Nombres, Apellidos, **Matricula**, Telefono, Email, **Activo**, EspecialidadId | Matrícula única |
| `HorarioOdontologo` | OdontologoId, DiaSemana, HoraInicio, HoraFin | Se borran con el odontólogo |
| `Cita` | PacienteId, OdontologoId, FechaHora, DuracionMinutos, Motivo, **Estado**, MotivoCancelacion | No se puede borrar un paciente/odontólogo con citas |
| `Tratamiento` | PacienteId, OdontologoId, CitaId?, **Tipo**, Fecha, Costo, Observaciones | No se puede borrar si tiene pagos |
| `Pago` | TratamientoId, Monto, FechaPago, Metodo, **NumeroComprobante** | Comprobante único |

Enums: `EstadoCita` (Programada, Reprogramada, Cancelada, Atendida), `TipoTratamiento` (LimpiezaDental,
Ortodoncia, Endodoncia, Extraccion), `MetodoPago` (Efectivo, Tarjeta, Transferencia, QR).

> **Base de datos y cambios de modelo:** se usa `EnsureCreated()` (sin migraciones) para evitar conflictos
> entre grupos. Si alguien agrega una columna/entidad, **borra `src/ClinicaDental.Web/clinica.db`** y vuelve
> a ejecutar: se recrea con los datos de ejemplo. Al cerrar el proyecto el líder puede generar una única migración.

---

## Módulo 3 — Gestión de Citas (AndresFundamentalistas) ✅

Implementado completo y usado como **implementación de referencia** para los demás grupos.

| Requisito | Pantalla | Servicio |
|-----------|----------|----------|
| Agendar citas | `/Citas/Create` (muestra horarios libres del odontólogo) | `AgendarAsync` |
| Reprogramar citas | `/Citas/Reprogramar/{id}` | `ReprogramarAsync` |
| Cancelar citas | `/Citas/Cancelar/{id}` (pide motivo) | `CancelarAsync` |
| Consultar agenda diaria | `/Citas/Agenda?fecha=yyyy-MM-dd` (filtro por odontólogo) | `ObtenerAgendaDiariaAsync` |
| Extra: listado con filtros | `/Citas` | `ListarAsync` |
| Extra: marcar atendida | botón en `/Citas/Details/{id}` (lo usa Tratamientos) | `MarcarAtendidaAsync` |
| Extra: horarios libres (JSON) | `/Citas/HorariosDisponibles?odontologoId=&fecha=&duracion=` | `ObtenerHorariosDisponiblesAsync` |

Reglas validadas al agendar/reprogramar (todas lanzan `ReglaNegocioException`):

1. El paciente existe. 2. El odontólogo existe y está **activo**. 3. La fecha/hora es **futura**.
4. Duración entre 15 y 240 min. 5. La cita cae **completa dentro de una franja** del horario del odontólogo.
6. El odontólogo no tiene otra cita no cancelada que se **solape**. 7. El paciente tampoco. 8. Motivo obligatorio.
Solo se reprograma/cancela/atiende una cita **Programada** o **Reprogramada**; cancelar libera el horario.

Pruebas: 52 unitarias (`Modulos/Citas`) + 17 de integración (`Integracion/PaginasCitasTests.cs`).

---

## Resumen de lo que verifica cada suite

| Suite | Qué garantiza |
|-------|---------------|
| `Base/ContratosTests` | Nadie cambió firmas de interfaces, ni renombró/borró propiedades de entidades o valores de enums |
| `Base/BaseDeDatosTests` | Índices únicos, claves foráneas, suma de decimales en SQLite, datos de ejemplo |
| `Integracion/PaginasBaseTests` | El menú enlaza los 5 módulos y los 5 servicios están registrados |
| `Modulos/<X>/<X>ServiceTests` | Las reglas de negocio del módulo (detalle en cada interfaz de `Services/Contratos/`) |
| `Integracion/Paginas<X>Tests` | Las rutas obligatorias del módulo responden 200 con el layout común, y 404 para ids inexistentes |
