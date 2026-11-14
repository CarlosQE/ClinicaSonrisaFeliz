# Parte A — Gestión de Odontólogos

Responsable: *(asignar)*

Alcance: registrar odontólogos, listarlos, buscarlos, actualizar sus datos y cambiar el estado activo/inactivo.

> Esta parte se puede integrar y probar sin la Parte B. La validación de que la especialidad existe se hace consultando directamente `BaseDatos`.

## Archivos

```
Parte A - Odontologos/
├─ DTOs/Odontologo/
│  ├─ OdontologoDto.cs
│  ├─ ActualizarOdontologoDto.cs
│  └─ CambiarEstadoOdontologoDto.cs
├─ Interfaces/IOdontologo.cs
├─ Services/OdontologoServicio.cs
├─ Controllers/OdontologoController.cs
└─ peticiones.http
```

## Integración

1. Copiar la carpeta `DTOs/Odontologo` a `src/ClinicaDental.Web/DTOs/`.
2. Copiar `IOdontologo.cs` a `src/ClinicaDental.Web/Interfaces/`.
3. Copiar `OdontologoServicio.cs` a `src/ClinicaDental.Web/Services/`.
4. Copiar `OdontologoController.cs` a `src/ClinicaDental.Web/Controllers/`.
5. En `Program.cs`, debajo del módulo 2, agregar:

   ```csharp
   builder.Services.AddScoped<IOdontologo, OdontologoServicio>();
   ```

6. Compilar y abrir Swagger.

No se necesita migración ni cambio en los modelos.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Odontologo` | Lista todos los odontólogos |
| GET | `/api/Odontologo?activo=true` | Filtra por estado |
| GET | `/api/Odontologo?idEspecialidad={guid}` | Filtra por especialidad |
| GET | `/api/Odontologo/{id}` | Busca uno por id |
| POST | `/api/Odontologo` | Registra un odontólogo |
| PUT | `/api/Odontologo/{id}` | Actualiza datos personales y especialidad |
| PUT | `/api/Odontologo/{id}/estado` | Activa o desactiva |

## Reglas implementadas

- Nombres, apellidos, matrícula y teléfono no pueden quedar vacíos ni con solo espacios.
- La matrícula es única. Al actualizar se excluye el mismo odontólogo.
- La especialidad debe existir.
- Todo odontólogo nuevo queda **activo**.
- No se puede desactivar un odontólogo con citas futuras no canceladas.

## Ejemplos de JSON

### Registrar — `POST /api/Odontologo`

```json
{
  "nombres": "Ana",
  "apellidos": "Rojas Pérez",
  "matricula": "MP-0010",
  "telefono": "70001234",
  "email": "ana.rojas@sonrisafeliz.bo",
  "idEspecialidad": "10000000-0000-0000-0000-000000000001"
}
```

### Actualizar — `PUT /api/Odontologo/{id}`

```json
{
  "nombres": "Ana",
  "apellidos": "Rojas Pérez",
  "matricula": "MP-0010",
  "telefono": "70001234",
  "email": "ana.rojas@sonrisafeliz.bo",
  "idEspecialidad": "10000000-0000-0000-0000-000000000001"
}
```

### Cambiar estado — `PUT /api/Odontologo/{id}/estado`

```json
{
  "activo": false
}
```

## Casos para probar en Swagger

- Registrar con matrícula repetida (`MP-0001`) → 400.
- Registrar con especialidad inexistente → 400.
- Registrar con nombres en blanco → 400.
- Actualizar manteniendo su propia matrícula → 200.
- Actualizar con la matrícula de otro odontólogo → 400.
- Desactivar un odontólogo con una cita futura → 400.
- Activar el odontólogo inactivo de ejemplo (`20000000-0000-0000-0000-000000000004`) → 200.
- Buscar un id inexistente → 404.

## Checklist antes del Pull Request

- [ ] Se copiaron solo los archivos de esta parte.
- [ ] `Program.cs` tiene únicamente la línea de `IOdontologo`.
- [ ] El proyecto compila sin errores.
- [ ] Se probaron los endpoints en Swagger, incluidos los casos de error.
- [ ] No se agregaron migraciones.
- [ ] No se modificaron modelos ni archivos de otros módulos.
