# Parte B — Especialidades y Horarios

Responsable: *(asignar)*

Alcance: CRUD de especialidades y gestión de los horarios de trabajo de los odontólogos.

> Esta parte se puede integrar y probar sin la Parte A. La validación de que el odontólogo existe se hace consultando directamente `BaseDatos`.

## Archivos

```
Parte B - Especialidades y Horarios/
├─ DTOs/
│  ├─ Especialidad/EspecialidadDto.cs
│  └─ HorarioOdontologo/HorarioOdontologoDto.cs
├─ Interfaces/
│  ├─ IEspecialidad.cs
│  └─ IHorarioOdontologo.cs
├─ Services/
│  ├─ EspecialidadServicio.cs
│  └─ HorarioOdontologoServicio.cs
├─ Controllers/
│  ├─ EspecialidadController.cs
│  └─ HorarioOdontologoController.cs
└─ peticiones.http
```

## Integración

1. Copiar `DTOs/Especialidad` y `DTOs/HorarioOdontologo` a `src/ClinicaDental.Web/DTOs/`.
2. Copiar `IEspecialidad.cs` e `IHorarioOdontologo.cs` a `src/ClinicaDental.Web/Interfaces/`.
3. Copiar `EspecialidadServicio.cs` y `HorarioOdontologoServicio.cs` a `src/ClinicaDental.Web/Services/`.
4. Copiar `EspecialidadController.cs` y `HorarioOdontologoController.cs` a `src/ClinicaDental.Web/Controllers/`.
5. En `Program.cs`, debajo del módulo 2, agregar:

   ```csharp
   builder.Services.AddScoped<IEspecialidad, EspecialidadServicio>();
   builder.Services.AddScoped<IHorarioOdontologo, HorarioOdontologoServicio>();
   ```

6. Compilar y abrir Swagger.

No se necesita migración ni cambio en los modelos.

## Endpoints de especialidades

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Especialidad` | Lista especialidades |
| GET | `/api/Especialidad/{id}` | Busca una especialidad |
| POST | `/api/Especialidad` | Registra una especialidad |
| PUT | `/api/Especialidad/{id}` | Actualiza una especialidad |
| DELETE | `/api/Especialidad/{id}` | Elimina si no tiene odontólogos |

## Endpoints de horarios

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/HorarioOdontologo/odontologo/{idOdontologo}` | Lista los horarios del odontólogo |
| GET | `/api/HorarioOdontologo/{id}` | Busca un horario |
| POST | `/api/HorarioOdontologo/odontologo/{idOdontologo}` | Agrega una franja |
| PUT | `/api/HorarioOdontologo/{id}` | Actualiza una franja |
| DELETE | `/api/HorarioOdontologo/{id}` | Elimina si no tiene citas futuras |

## Reglas implementadas

### Especialidades

- El nombre es obligatorio y único (sin distinguir mayúsculas/minúsculas).
- No se puede eliminar una especialidad que tenga odontólogos asignados.

### Horarios

- El odontólogo debe existir.
- `HoraFin` debe ser mayor que `HoraInicio`.
- No se permiten franjas solapadas del mismo odontólogo en el mismo día.
- No se puede modificar ni eliminar una franja que tenga citas futuras.

## Ejemplos de JSON

### Registrar especialidad — `POST /api/Especialidad`

```json
{
  "nombre": "Periodoncia",
  "descripcion": "Tratamiento de encías"
}
```

### Registrar horario — `POST /api/HorarioOdontologo/odontologo/{idOdontologo}`

```json
{
  "diaSemana": 1,
  "horaInicio": "08:00:00",
  "horaFin": "12:00:00"
}
```

`diaSemana`: Domingo = 0, Lunes = 1, Martes = 2, Miércoles = 3, Jueves = 4, Viernes = 5, Sábado = 6.

## Casos para probar en Swagger

- Registrar especialidad repetida (`Ortodoncia`) → 400.
- Registrar especialidad con nombre en blanco → 400.
- Actualizar una especialidad manteniendo su propio nombre → 200.
- Eliminar una especialidad en uso (`10000000-0000-0000-0000-000000000001`) → 400.
- Registrar horario solapado (lunes 09:00–11:00) → 400.
- Registrar horario con `horaFin` menor que `horaInicio` → 400.
- Registrar horario para un odontólogo inexistente → 404.
- Eliminar un horario con citas futuras → 400.
- Buscar un id inexistente → 404.

## Checklist antes del Pull Request

- [ ] Se copiaron solo los archivos de esta parte.
- [ ] `Program.cs` tiene únicamente las líneas de `IEspecialidad` e `IHorarioOdontologo`.
- [ ] El proyecto compila sin errores.
- [ ] Se probaron los endpoints en Swagger, incluidos los casos de error.
- [ ] No se agregaron migraciones.
- [ ] No se modificaron modelos ni archivos de otros módulos.
