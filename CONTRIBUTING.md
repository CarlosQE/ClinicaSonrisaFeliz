# Guía de contribución

Esta guía describe el flujo de trabajo y las convenciones para colaborar en el repositorio de la Clínica Dental Sonrisa Feliz.
Los 5 módulos están implementados (Pacientes, Odontólogos/Especialidades/Horarios, Citas, Tratamientos y Pagos); cualquier cambio nuevo debe integrarse respetando la estructura y reglas vigentes.

---

## 1. Preparar el entorno (una sola vez)

1. Instala lo indicado en el [README](README.md#requisitos): .NET SDK 10, Visual Studio, SQL Server LocalDB y Git.
2. Clona el repositorio y crea una rama de trabajo a partir de `main` actualizado:

   ```bash
   git checkout main
   git pull origin main
   git checkout -b <tipo>/<nombre-corto>
   ```

3. Crea o actualiza tu base de datos local con `Update-Database` (Consola del Administrador de paquetes, proyecto `ClinicaDental.Web`) y ejecuta el proyecto.
4. Verifica en Swagger que los endpoints respondan correctamente.

## 2. Estado de módulos y ramas

| Módulo | Estado | Rama de trabajo (histórica/grupo) |
|---|---|---|
| Pacientes | Implementado | `modulo/pacientes` |
| Odontólogos, Especialidades y Horarios | Implementado | `modulo/odontologos` |
| Citas | Implementado | `modulo/citas` |
| Tratamientos | Implementado | `modulo/tratamientos` |
| Pagos | Implementado | `modulo/pagos` |

Para nuevas tareas o correcciones, usa ramas descriptivas (por ejemplo `fix/...`, `docs/...`, `refactor/...`) desde `main` actualizado.

## 3. Estructura y alcance de cambios

Estructura vigente del proyecto web (`src/ClinicaDental.Web/`): `Controllers/`, `DTOs/`, `Data/`, `Factories/`, `Interfaces/`, `Migrations/`, `Models/`, `Services/`, además de `Program.cs` y configuración (`appsettings*.json`).

- Mantén la separación: **DTOs** (entrada), **Interfaces** (contratos), **Services** (reglas/consultas), **Controllers** (validación de entrada HTTP y respuestas).
- Cambia solo lo necesario para la tarea y evita tocar archivos de módulos no relacionados.
- Cualquier modificación de impacto transversal (modelo compartido, DbContext, configuración global, CI, solución) debe coordinarse antes.

## 4. Convenciones de implementación

- Nombres en **español** y coherentes con el resto del proyecto.
- `Id` de tipo `Guid`; generación con `Guid.NewGuid()` en el servicio al crear.
- Rutas y atributos: `[Route("api/[controller]")]`, `[ApiController]`, y `Name` en cada verbo HTTP.
- Validar `ModelState` en `POST/PUT`, aplicar validaciones de negocio del servicio y responder con `BadRequest`/`NotFound` claros; envolver operaciones con `try/catch` y responder `500` ante errores inesperados.
- Lógica de negocio y consultas en el **servicio** (no en el controlador).
- Hora actual mediante `TimeProvider` inyectado (no `DateTime.Now`).
- Sin repositorios genéricos, MediatR, AutoMapper ni Identity/JWT.

## 5. Base de datos y migraciones (regla vigente)

- La base se gestiona con migraciones de EF Core en `Migrations/`.
- **Los grupos no commitean migraciones; las genera el líder.**
- Si un cambio requiere modificar el modelo:
  1. Realiza solo el ajuste necesario y coordínalo.
  2. Para pruebas locales puedes generar/aplicar una migración temporal, pero **no la incluyas en el commit**.
  3. El líder genera y publica la migración oficial en `main` tras integrar el cambio.
- Después de cada integración que incluya migraciones nuevas, ejecuta `Update-Database`.
- Si tu base local queda inconsistente: `Drop-Database` y luego `Update-Database` (se recrea con el seed).

## 6. Flujo para entregar cambios (Pull Request)

1. Actualiza tu rama con lo último de `main` y resuelve conflictos si los hay:

   ```bash
   git fetch origin
   git merge origin/main
   ```

2. Comprueba que el proyecto compila y que los endpoints afectados funcionan en Swagger.
3. Sube tu rama y abre un **Pull Request hacia `main`**. Describe claramente el cambio y su alcance.
4. Verifica los checks de CI (compilación). Atiende observaciones de revisión.

### Definición de "terminado"

- [ ] El proyecto compila sin errores.
- [ ] Los endpoints afectados funcionan en Swagger (casos correctos y de error).
- [ ] Solo se modificaron archivos necesarios y acordes al alcance.
- [ ] No se incluyeron migraciones en el commit (salvo que el líder lo indique expresamente).
- [ ] La documentación relevante se actualizó si el cambio lo requiere.

## 7. Notas

- El proyecto `tests/` está fuera de la solución y se reescribirá para la API; no lo uses como referencia de endpoints actuales.
- Ante dudas sobre cambios en archivos compartidos (`Data/BaseDatos.cs`, `Migrations/*`, configuración global, `.sln`), coordina antes de modificar.
