# Guía para los grupos — cómo implementar tu módulo

Esta guía explica **exactamente** qué tiene que hacer cada grupo para que su parte se una sin problemas
al programa final. La regla de oro:

> **Tu módulo está terminado cuando `dotnet test --filter "Modulo=<TuModulo>"` sale en verde
> y `dotnet test --filter "Modulo=Base"` sigue en verde.**

Las pruebas ya están escritas. **No las modifiques para que pasen**: modifica tu código.

---

## 1. Preparar el entorno (una sola vez)

1. Instala el **.NET SDK 10** y comprueba con `dotnet --version`.
2. Clona el repositorio y crea **la rama de tu grupo** a partir de `main`:

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

3. Compila, ejecuta y mira las pruebas de tu módulo fallar (es lo esperado al inicio):

   ```bash
   dotnet build
   ```

   ```bash
   dotnet test --filter "Modulo=Pacientes"
   ```

---

## 2. ¿Qué archivos puede tocar cada grupo?

Trabajar solo en "tus" archivos evita conflictos de Git al unir las ramas.

| Grupo | Puede crear / modificar |
|-------|-------------------------|
| ApostolesEvoMorales | `Services/PacienteService.cs`, `Controllers/PacientesController.cs`, `Views/Pacientes/*`, `ViewModels/Paciente*.cs` |
| BagayerosChaco | `Services/OdontologoService.cs`, `Controllers/OdontologosController.cs`, `Views/Odontologos/*`, `ViewModels/Odontologo*.cs` |
| AndresFundamentalistas | `Services/CitaService.cs`, `Controllers/CitasController.cs`, `Views/Citas/*`, `ViewModels/CitaViewModels.cs` |
| Ultimo | `Services/TratamientoService.cs`, `Controllers/TratamientosController.cs`, `Views/Tratamientos/*`, `ViewModels/Tratamiento*.cs` |
| GuaposMisteriosos | `Services/PagoService.cs`, `Controllers/PagosController.cs`, `Views/Pagos/*`, `ViewModels/Pago*.cs` |

Todos pueden **agregar** pruebas nuevas en `tests/ClinicaDental.Tests/Modulos/<SuModulo>/` (en un archivo nuevo).

### 🚫 Archivos que NO se tocan sin hablar con el líder

| Archivo | Por qué |
|---------|---------|
| `Services/Contratos/I*.cs` | Otros módulos y las pruebas dependen de estas firmas. Hay un test (`ContratosTests`) que falla si cambian. |
| `Models/*.cs` (renombrar o borrar propiedades) | Otros módulos las usan. **Agregar** propiedades nuevas a la entidad de tu módulo sí se permite (avisa en el PR). |
| `Data/ClinicaDbContext.cs`, `Data/DatosIniciales.cs` | Modelo y datos compartidos. |
| `Program.cs`, `Common/*` | Configuración global. Tu servicio **ya está registrado** en `Common/RegistroServicios.cs`. |
| `Views/Shared/_Layout.cshtml` | El menú ya enlaza a los 5 módulos. |
| `tests/.../Infraestructura/*`, `tests/.../Base/*`, pruebas de otros módulos | Son la referencia común. |

¿Necesitas cambiar algo de esta lista? Abre un *Issue* o escribe al líder explicando qué y por qué.
Si se aprueba, el líder hace el cambio en `main` y todos actualizan su rama (`git merge main`).

---

## 3. Pasos para implementar tu módulo

### Paso 1 — Lee tu contrato

Abre `src/ClinicaDental.Web/Services/Contratos/I<TuModulo>Service.cs`. Cada método tiene comentarios que
dicen **qué debe hacer, qué reglas validar y qué excepción lanzar**. Las pruebas de
`tests/ClinicaDental.Tests/Modulos/<TuModulo>/` verifican exactamente eso.

### Paso 2 — Implementa el servicio

Abre `Services/<TuModulo>Service.cs` y reemplaza cada `throw new NotImplementedException(...)`.
Ve corriendo tus pruebas a medida que avanzas. Ejemplo de estructura (tomado de `CitaService.cs`):

```csharp
public async Task CancelarAsync(int citaId, string motivo)
{
    // 1. Buscar. Si no existe -> NoEncontradoException
    var cita = await _db.Citas.FirstOrDefaultAsync(c => c.Id == citaId)
        ?? throw new NoEncontradoException("Cita", citaId);

    // 2. Validar reglas de negocio. Si se viola alguna -> ReglaNegocioException con mensaje claro
    if (!cita.EstaVigente)
        throw new ReglaNegocioException($"No se puede cancelar una cita en estado {cita.Estado}.");
    if (string.IsNullOrWhiteSpace(motivo))
        throw new ReglaNegocioException("Debe indicar el motivo de la cancelación.");

    // 3. Modificar y guardar
    cita.Estado = EstadoCita.Cancelada;
    cita.MotivoCancelacion = motivo.Trim();
    await _db.SaveChangesAsync();
}
```

Reglas importantes:

- **Solo dos excepciones**: `ReglaNegocioException` (regla violada) y `NoEncontradoException` (id inexistente).
  No lances `Exception`, `ArgumentException`, etc.: las pruebas no las reconocerán.
- **"Ahora" se obtiene con `TimeProvider`**, nunca con `DateTime.Now`:
  `var ahora = _reloj.GetLocalNow().DateTime;` (las pruebas "congelan" el reloj en el lunes 07/01/2030 08:00).
- **Consultas de lectura con `AsNoTracking()`** y los `Include(...)` que pida el contrato
  (por ejemplo "devuelve el pago con Tratamiento y Tratamiento.Paciente").
- **Búsquedas sin distinguir mayúsculas**: en SQLite `Contains` distingue mayúsculas; usa
  `x.Nombres.ToLower().Contains(texto.ToLower())`.
- Puedes usar los datos de otros módulos **leyendo directamente** del `ClinicaDbContext`
  (así no dependes de que el otro grupo termine). Si necesitas una **acción** de otro módulo, usa su interfaz
  (ejemplo: `TratamientoService` recibe `ICitaService` para marcar la cita como atendida).

### Paso 3 — Controlador y vistas

Tu controlador ya existe con la acción `Index`. Agrega las demás acciones y vistas siguiendo
`CitasController.cs` (es la referencia). El patrón estándar es:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(MiViewModel vm)
{
    if (ModelState.IsValid)
    {
        try
        {
            await _servicio.RegistrarAsync(/* ... */);
            TempData["Exito"] = "Registro guardado.";          // mensaje verde (lo muestra el layout)
            return RedirectToAction(nameof(Index));            // patrón Post-Redirect-Get
        }
        catch (ReglaNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message); // se ve en asp-validation-summary
        }
    }
    return View(vm);
}
```

- `NoEncontradoException` **no hace falta capturarla**: el filtro global la convierte en 404.
- En acciones GET con id, si el servicio devuelve `null` → `return NotFound();`.
- Para mensajes fuera de formularios: `TempData["Exito"]` o `TempData["Error"]`.
- Vistas con **Bootstrap 5** (ya incluido) y `@{ ViewData["Title"] = "..."; }`. No cambies el layout.
- Fechas en formularios: `<input type="date">` / `<input type="datetime-local">`; montos con `<input type="number" step="0.01">`
  (el sistema usa punto decimal en todas las PCs).

### Paso 4 — Rutas obligatorias (las prueban los tests de integración)

| Módulo | Rutas que deben responder **200** | Deben responder **404** si el id no existe |
|--------|-----------------------------------|--------------------------------------------|
| Pacientes | `/Pacientes`, `/Pacientes?busqueda=`, `/Pacientes/Create`, `/Pacientes/Edit/{id}`, `/Pacientes/Historial/{id}` | Edit, Historial |
| Odontólogos | `/Odontologos`, `/Odontologos/Create`, `/Odontologos/Edit/{id}`, `/Odontologos/Horarios/{id}`, `/Odontologos/Especialidades` | Edit, Horarios |
| Citas ✅ | `/Citas`, `/Citas/Agenda`, `/Citas/Create`, `/Citas/Details/{id}`, `/Citas/Reprogramar/{id}`, `/Citas/Cancelar/{id}` | Details, Reprogramar, Cancelar |
| Tratamientos | `/Tratamientos`, `/Tratamientos?pacienteId=`, `/Tratamientos/Create`, `/Tratamientos/Create?citaId=`, `/Tratamientos/Details/{id}`, `/Tratamientos/Observaciones/{id}` | Details, Observaciones |
| Pagos | `/Pagos`, `/Pagos?pacienteId=`, `/Pagos/Create`, `/Pagos/Create?tratamientoId=`, `/Pagos/Deudas`, `/Pagos/Comprobante/{id}` | Comprobante |

Todas deben usar el layout común (no pongas `Layout = null`).

### Paso 5 — Prueba a mano

```bash
dotnet run --project src/ClinicaDental.Web
```

Usa los datos de ejemplo. Si cambiaste alguna entidad, borra `src/ClinicaDental.Web/clinica.db` antes.

---

## 4. Ejecutar y leer las pruebas

| Comando | Para qué |
|---------|----------|
| `dotnet test --filter "Modulo=Pacientes"` | Todas las pruebas de tu módulo (unitarias + integración) |
| `dotnet test --filter "Modulo=Pacientes&Tipo!=Integracion"` | Solo las del servicio (más rápido mientras programas) |
| `dotnet test --filter "Modulo=Base"` | Que no rompiste nada común |
| `dotnet test --filter "FullyQualifiedName~Registrar_CIDuplicado"` | Una prueba puntual |
| `dotnet test` | Todo el sistema |

Cada prueba tiene un nombre que explica el caso: `Metodo_Situacion_ResultadoEsperado`.
Ejemplo: `Eliminar_ConCitas_LanzaReglaNegocio` → "si elimino un paciente que tiene citas, debe lanzar `ReglaNegocioException`".

Mensajes típicos y qué significan:

| Mensaje | Causa probable |
|---------|----------------|
| `System.NotImplementedException` | Aún no implementaste ese método. |
| `Assert.Throws() Failure: No exception was thrown` | Falta validar una regla. |
| `Assert.Throws() Failure: Exception type was not an exact match` | Lanzaste otra excepción; usa `ReglaNegocioException`/`NoEncontradoException`. |
| `GET /Pacientes/Edit/1 devolvió 501` | La página llama a un método del servicio que todavía no implementaste. |
| `GET ... devolvió 404` | Falta la acción del controlador o la vista con ese nombre. |
| `Assert.NotNull() Failure` en una propiedad de navegación | Te faltó un `.Include(...)`. |
| Falla en `ContratosTests` | Cambiaste una firma o renombraste una propiedad del contrato: revierte el cambio. |

### Agregar tus propias pruebas (recomendado)

Crea un archivo nuevo en `tests/ClinicaDental.Tests/Modulos/<TuModulo>/` y marca la clase con el rasgo de tu módulo.
Usa la infraestructura común:

```csharp
[Trait("Modulo", Modulo.Pacientes)]
public class MisPruebasExtraTests : IDisposable
{
    private readonly BaseDatosPrueba _bd = new();     // SQLite en memoria, vacía
    public void Dispose() => _bd.Dispose();

    [Fact]
    public async Task MiCaso()
    {
        var e = _bd.Escenario();                       // crea datos sin depender de otros módulos
        var (paciente, odontologo) = await e.PacienteYOdontologoAsync();
        var servicio = new PacienteService(_bd.NuevoContexto(), _bd.Reloj);
        // ... actuar y verificar con Assert
    }
}
```

`Escenario` tiene: `PacienteAsync`, `OdontologoAsync` (horario L‑V 08‑12 y 14‑18), `EspecialidadAsync`,
`CitaAsync`, `TratamientoAsync`, `PagoAsync`, `ProximoLunesA(hora, minuto)`.

---

## 5. Entregar tu módulo (Pull Request)

1. Actualiza tu rama con lo último de `main` y resuelve conflictos si los hay:

   ```bash
   git fetch origin
   ```

   ```bash
   git merge origin/main
   ```

2. Comprueba que todo está bien:

   ```bash
   dotnet test --filter "Modulo=Base"
   ```

   ```bash
   dotnet test --filter "Modulo=Pacientes"
   ```

3. Sube tu rama y abre un **Pull Request hacia `main`**. Completa el checklist de la plantilla.
4. GitHub Actions corre las pruebas de cada módulo por separado (pestaña *Checks*). El líder revisa y une.

### Definición de "terminado" ✅

- [ ] Todas las pruebas de `Modulo=<TuModulo>` en verde.
- [ ] `Modulo=Base` en verde.
- [ ] Las pruebas de los módulos que ya estaban en verde siguen en verde.
- [ ] Solo se modificaron archivos permitidos para tu grupo.
- [ ] Las pantallas funcionan a mano con los datos de ejemplo y muestran los errores de negocio en el formulario.

---

## 6. Convenciones de código

- Nombres en **español** (clases, métodos, variables), igual que el resto del proyecto.
- Métodos asíncronos con sufijo `Async` y `await` (no uses `.Result` ni `.Wait()`).
- Campos privados con guion bajo: `_db`, `_reloj`.
- Lógica de negocio **solo en el servicio**; el controlador no consulta reglas ni hace cálculos.
- Nada de `DateTime.Now`: usa `TimeProvider`.
- Mensajes de error para el usuario: claros y en español ("El CI ya está registrado.").

## 7. Preguntas frecuentes

**¿Puedo usar datos de pacientes u odontólogos si esos módulos no están listos?**
Sí. Los datos de ejemplo ya existen en la base, y tu servicio puede leerlos del `ClinicaDbContext`.

**Agregué una propiedad a una entidad y la app falla con "no such column".**
Borra `src/ClinicaDental.Web/clinica.db` y vuelve a ejecutar.

**Las pruebas pasan en mi PC pero fallan en GitHub.**
Probablemente usaste `DateTime.Now` o dependes del orden de los datos sin `OrderBy`.

**Necesito un método nuevo en mi interfaz.**
Puedes **agregarlo** (sin cambiar los existentes) avisando en el PR; luego el líder actualiza `ContratosTests`.
