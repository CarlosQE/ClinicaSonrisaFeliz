# Gestión de Odontólogos — Módulo 2 dividido en 2 partes

Carpeta de trabajo del módulo **Gestión de Odontólogos** (grupo *BagayerosChaco*), separada en dos partes independientes para repartir el trabajo.

> Esta carpeta **no forma parte de la solución** ni se compila todavía. Es solo el código del módulo para revisarlo con el equipo y copiarlo a `src/ClinicaDental.Web` cuando esté aprobado.

## División del trabajo

| Parte | Responsable | Alcance |
|---|---|---|
| **Parte A – Odontólogos** | *(asignar)* | Registrar, listar, buscar, actualizar odontólogos y cambiar su estado activo/inactivo |
| **Parte B – Especialidades y Horarios** | *(asignar)* | CRUD de especialidades y gestión de horarios de trabajo |

Cada parte es independiente: se puede integrar y probar sola. Las validaciones que necesitan datos de la otra parte se hacen consultando directamente `BaseDatos`, sin depender del servicio del compañero.

## Estructura

```
Gestion de odontologos/
├─ LEEME.md
├─ Parte A - Odontologos/
│  ├─ LEEME.md
│  ├─ peticiones.http
│  ├─ DTOs/Odontologo/
│  │  ├─ OdontologoDto.cs
│  │  ├─ ActualizarOdontologoDto.cs
│  │  └─ CambiarEstadoOdontologoDto.cs
│  ├─ Interfaces/
│  │  └─ IOdontologo.cs
│  ├─ Services/
│  │  └─ OdontologoServicio.cs
│  └─ Controllers/
│     └─ OdontologoController.cs
└─ Parte B - Especialidades y Horarios/
   ├─ LEEME.md
   ├─ peticiones.http
   ├─ DTOs/
   │  ├─ Especialidad/
   │  │  └─ EspecialidadDto.cs
   │  └─ HorarioOdontologo/
   │     └─ HorarioOdontologoDto.cs
   ├─ Interfaces/
   │  ├─ IEspecialidad.cs
   │  └─ IHorarioOdontologo.cs
   ├─ Services/
   │  ├─ EspecialidadServicio.cs
   │  └─ HorarioOdontologoServicio.cs
   └─ Controllers/
      ├─ EspecialidadController.cs
      └─ HorarioOdontologoController.cs
```

## Modelos compartidos (ya existen)

No se crean modelos nuevos. Ya están en `src/ClinicaDental.Web/Models`:

| Modelo | Campos principales |
|---|---|
| `Odontologo` | `Id`, `Nombres`, `Apellidos`, `Matricula`, `Telefono`, `Email`, `Activo`, `IdEspecialidad` |
| `Especialidad` | `Id`, `Nombre`, `Descripcion` |
| `HorarioOdontologo` | `Id`, `IdOdontologo`, `DiaSemana`, `HoraInicio`, `HoraFin` |

Tampoco se tocan `BaseDatos`, `DatosIniciales`, `Migrations`, `appsettings`, `.csproj` ni `.sln`. No se necesitan migraciones nuevas.

## Integración conjunta

1. Copiar los archivos de la Parte A a `src/ClinicaDental.Web` siguiendo su `LEEME.md`.
2. Copiar los archivos de la Parte B siguiendo su `LEEME.md`.
3. En `Program.cs`, agregar debajo del módulo 2:

   ```csharp
   builder.Services.AddScoped<IOdontologo, OdontologoServicio>();
   builder.Services.AddScoped<IEspecialidad, EspecialidadServicio>();
   builder.Services.AddScoped<IHorarioOdontologo, HorarioOdontologoServicio>();
   ```

4. Compilar y probar en Swagger.

## Datos de ejemplo

| Qué | Ids |
|---|---|
| Especialidades | `10000000-0000-0000-0000-000000000001` … `…0004` |
| Odontólogos | `20000000-0000-0000-0000-000000000001` … `…0004` |
| Odontólogo inactivo | `20000000-0000-0000-0000-000000000004` |
| Horarios | Lunes a viernes, 08:00–12:00 y 14:00–18:00 |

Cada `LEEME.md` de las partes incluye sus propios endpoints, reglas, ejemplos de JSON y casos de prueba.
