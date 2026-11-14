# Guía para los grupos — cómo implementar tu módulo

Esta guía explica qué tiene que hacer cada grupo para que su parte se una sin problemas al programa final.
**Tu módulo de ejemplo es Citas**: copia su estructura (`DTOs/Cita/`, `Interfaces/ICita.cs`,
`Services/CitaServicio.cs`, `Controllers/CitaController.cs`).

---

## 1. Preparar el entorno (una sola vez)

1. Instala lo que pide el [README](README.md#requisitos): .NET SDK 10, Visual Studio, SQL Server LocalDB y Git.
2. Pide a **CarlosQE** que te agregue como colaborador del repositorio y acepta la invitación que llega a tu correo
   (necesitas permiso para subir tu rama).
3. Clona el repositorio y crea **la rama de tu grupo** a partir de `main`:

   | Grupo | Rama |
   |-------|------|
   | ApostolesEvoMorales | `modulo/pacientes` |
   | BagayerosChaco | `modulo/odontologos` |
   | AndresFundamentalistas | `modulo/citas` |
   | Ultimo | `modulo/tratamientos` |
   | GuaposMisteriosos | `modulo/pagos` |

   ```bash
   git checkout -b modulo/pacientes
   ```

4. Crea tu base de datos local con `Update-Database` (ver README, "Primeros pasos") y ejecuta el proyecto.
   Debe abrir Swagger con el endpoint de `Cita`.

---

## 2. ¿Qué archivos puede tocar cada grupo?

Trabajar solo en "tus" archivos evita conflictos de Git al unir las ramas.

| Grupo | Crea / modifica |
|-------|-----------------|
| ApostolesEvoMorales | `DTOs/Paciente/*`, `Interfaces/IPaciente.cs`, `Services/PacienteServicio.cs`, `Controllers/PacienteController.cs` |
| BagayerosChaco | `DTOs/Odontologo/*`, `DTOs/Especialidad/*`, `Interfaces/IOdontologo.cs`, `Services/OdontologoServicio.cs`, `Controllers/OdontologoController.cs` (y los de Especialidad / Horarios si los separan) |
| AndresFundamentalistas | `DTOs/Cita/*`, `Interfaces/ICita.cs`, `Services/CitaServicio.cs`, `Controllers/CitaController.cs` |
| Ultimo | `DTOs/Tratamiento/*`, `Interfaces/ITratamiento.cs`, `Services/TratamientoServicio.cs`, `Controllers/TratamientoController.cs` |
| GuaposMisteriosos | `DTOs/Pago/*`, `Interfaces/IPago.cs`, `Services/PagoServicio.cs`, `Controllers/PagoController.cs` |

Además, cada grupo puede:
- Descomentar **solo su línea** en `Program.cs` (sección "Inyección de dependencias de servicios").
- **Agregar** propiedades a **su** entidad en `Models/` (avisando en el PR, ver sección 5).

### 🚫 Archivos que NO se tocan sin hablar con el líder

| Archivo | Por qué |
|---------|---------|
| `Models/*.cs` de otros módulos (renombrar o borrar propiedades) | Otros módulos las usan. |
| `Data/BaseDatos.cs`, `Data/DatosIniciales.cs` | Modelo y datos compartidos. |
| `Migrations/*` | Solo el líder las genera (ver sección 5). |
| `appsettings.json`, `ClinicaDental.Web.csproj`, `ClinicaDental.sln` | Configuración global. |
| `.github/*`, `tests/*` | Proceso del curso. |
| Archivos de otros grupos | Evita conflictos. |

¿Necesitas cambiar algo de esta lista? Abre un *Issue* o escribe al líder explicando qué y por qué.

---

## 3. Pasos para implementar tu módulo

### Paso 1 — Revisa tu entidad y los datos de ejemplo
Tu entidad ya existe en `Models/` y su `DbSet` ya está en `Data/BaseDatos.cs`. Mira el diagrama del
[README](README.md#modelo-de-datos) y los Ids de ejemplo para probar sin crear datos a mano.

### Paso 2 — DTOs
En `DTOs/<Entidad>/` crea un DTO con **solo los campos que el cliente envía** (sin `Id`), con sus
validaciones básicas. Ejemplo: `DTOs/Cita/CitaDto.cs`.

### Paso 3 — Interfaz
En `Interfaces/I<Entidad>.cs` declara los métodos que usará el controlador: listar, buscar por id, crear,
actualizar/eliminar si tu módulo lo pide, y las **validaciones** como métodos `bool`
(`ExisteX`, `NombreRepetido`, `StockValido`...). Ejemplo: `Interfaces/ICita.cs`.

### Paso 4 — Servicio
En `Services/<Entidad>Servicio.cs` implementa la interfaz usando `BaseDatos`:

- Recibe `BaseDatos` por constructor (y `TimeProvider` si necesitas la hora actual; **nunca `DateTime.Now`**).
- Las consultas y las reglas de negocio viven **aquí**.
- Las validaciones devuelven `bool`; **no lanzan excepciones** para reglas de negocio.
- Usa `.Include(...)` cuando el resultado deba traer entidades relacionadas.
- Para leer datos de otros módulos, consulta directamente `BaseDatos` (no dependas de que el otro grupo termine).

### Paso 5 — Controlador
En `Controllers/<Entidad>Controller.cs`, sigue el patrón de `CitaController.cs`:

- `[Route("api/[controller]")]` y `[ApiController]`.
- **`Name` en cada `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`**.
- En los `POST`/`PUT`: primero `ModelState.IsValid`, luego las validaciones del servicio
  (cada una con su `BadRequest("mensaje claro")`), y al final `try/catch` con `StatusCode(500, ...)`.
- `GET` por id: `NotFound(...)` si no existe. `POST`: `CreatedAtAction(nameof(Get), new { id = ... }, nuevo)`.

### Paso 6 — Registra tu servicio
En `Program.cs`, descomenta **solo la línea de tu módulo**:
```csharp
builder.Services.AddScoped<IPaciente, PacienteServicio>();
```

### Paso 7 — Prueba en Swagger
Ejecuta el proyecto y prueba **cada endpoint**, con casos correctos y con casos de error
(dato duplicado, id inexistente, campo faltante, regla violada). Usa los Ids de ejemplo del README.

---

## 4. Endpoints sugeridos por módulo

Cada grupo decide los detalles, pero estos endpoints cubren los requisitos del enunciado:

| Módulo | Endpoints sugeridos |
|--------|---------------------|
| Pacientes | `GET /api/Paciente` · `GET /api/Paciente/{id}` · `POST` · `PUT /{id}` (actualizar datos) · `DELETE /{id}` (validar que no tenga citas ni tratamientos) · `GET /{id}/historial` (citas y tratamientos del paciente) |
| Odontólogos | `GET/POST /api/Odontologo` · `GET /{id}` · `PUT /{id}` · cambiar estado activo/inactivo · `GET/POST /api/Especialidad` · horarios del odontólogo (listar y agregar franjas) |
| Citas ✅ | Ver README |
| Tratamientos | `GET /api/Tratamiento` (filtrar por paciente y tipo) · `GET /{id}` · `POST` · registrar/actualizar observaciones |
| Pagos | `GET /api/Pago` · `GET /{id}` · `POST` (valida que no se pague más que la deuda) · deudas pendientes por paciente · comprobante de un pago |

Dependencias entre módulos:
- **Tratamientos** usa pacientes, odontólogos y (opcionalmente) citas. Si necesitan **marcar una cita como atendida**,
  pídanlo al grupo AndresFundamentalistas.
- **Pagos** usa tratamientos. Hay 2 tratamientos y 1 pago de ejemplo en la base para probar.

---

## 5. Base de datos y migraciones

La base se crea con migraciones (`Migrations/`). Para evitar conflictos entre grupos:

- **No subas migraciones en tu Pull Request.** Si dos ramas generan migraciones distintas, chocan en `BaseDatosModelSnapshot.cs`.
- Si necesitas **agregar una propiedad a tu entidad**:
  1. Agrégala en tu modelo (solo agregar; no renombres ni borres).
  2. Avísalo en el PR (sección "Cambios en entidades").
  3. Para probarla en tu PC puedes generar una migración local con `Add-Migration MiCambio` y `Update-Database`,
     pero **no incluyas ese archivo en tu commit** (bórralo antes de subir y deja solo el cambio en el modelo).
  4. El **grupo líder** genera la migración oficial en `main` después del merge.
- Después de cada `git merge origin/main` donde entren migraciones nuevas, ejecuta `Update-Database`.
- Si tu base local queda inconsistente: `Drop-Database` y luego `Update-Database` (se recrea con los datos de ejemplo).

---

## 6. Convenciones de código

- Nombres en **español** (clases, métodos, variables), igual que el resto del proyecto.
- `Id` siempre `Guid`; se genera con `Guid.NewGuid()` en el servicio al crear.
- Estructura: `DTOs/<Entidad>/`, `Interfaces/I<Entidad>.cs`, `Services/<Entidad>Servicio.cs`, `Controllers/<Entidad>Controller.cs`.
- Campos privados con guion bajo: `_context`, `_reloj`.
- Lógica de negocio **solo en el servicio**; el controlador solo coordina y responde.
- Validaciones como métodos `bool` en el servicio; el controlador responde `BadRequest`.
- Evita atributos de validación que no se hayan visto en clase (por ejemplo `[Range]`): esas reglas van en el servicio.
- Decimales (montos): `[Column(TypeName = "decimal(18,2)")]` en el modelo.
- Mensajes de error para el usuario: claros y en español ("El CI ya está registrado.").
- Nada de `DateTime.Now`: usa `TimeProvider`.

---

## 7. Entregar tu módulo (Pull Request)

1. Actualiza tu rama con lo último de `main` y resuelve conflictos si los hay:

   ```bash
   git fetch origin
   ```

   ```bash
   git merge origin/main
   ```

2. Comprueba que compila y que todo funciona en Swagger.
3. Sube tu rama y abre un **Pull Request hacia `main`**. Completa el checklist de la plantilla.
4. GitHub Actions compila el proyecto (pestaña *Checks*). El líder revisa y une.

### Definición de "terminado" ✅

- [ ] El proyecto compila sin errores ni warnings.
- [ ] Todos los requisitos de tu módulo (tabla del README) tienen su endpoint y funcionan en Swagger.
- [ ] Probaste los casos de error (dato duplicado, id inexistente, regla violada, campo faltante).
- [ ] Solo se modificaron archivos permitidos para tu grupo y tu línea de `Program.cs`.
- [ ] No subiste migraciones.

> **Pruebas automáticas:** `tests/` está en reescritura para probar los endpoints. Cuando estén publicadas,
> esta guía se actualizará con cómo correrlas.

---

## 8. Preguntas frecuentes

**¿Puedo usar datos de pacientes u odontólogos si esos módulos no están listos?**
Sí. Los datos de ejemplo ya están en la base y tu servicio puede leerlos de `BaseDatos`.

**`Update-Database` falla o la base quedó mal.**
Ejecuta `Drop-Database` y luego `Update-Database`.

**Error "Build failed" al ejecutar comandos de la consola.**
Abre *Ver → Lista de errores* y corrige el error de compilación; los comandos de migración no corren con el proyecto roto.

**Necesito un método nuevo en mi interfaz.**
Es tu interfaz: agrégalo libremente. Solo avisa si otro módulo la va a usar.

**Dos grupos modificaron `Program.cs`.**
Cada uno tiene su propia línea comentada; al hacer el merge conserva **ambas** líneas.
