# Clínica Dental Sonrisa Feliz

Sistema web para administrar pacientes, odontólogos, especialidades, horarios, citas, tratamientos y pagos
de la Clínica Dental Sonrisa Feliz. Tiene tres proyectos (mismo esquema que el ejemplo de clase *Actividad1*):

| Proyecto | Qué es | Puerto (perfil `http`) |
|---|---|---|
| `src/ClinicaDental.Web` | **Backend**: API REST en ASP.NET Core Web API (.NET 10) + Entity Framework Core + **SQL Server** | `http://localhost:5087` (Swagger en `/swagger`) |
| `src/ClinicaDental.ClienteBlazor` | **Frontend**: Blazor Web App (componentes `.razor`, interactivo en servidor) que consume la API con `HttpClient` y token JWT | `http://localhost:5180` |
| `src/ClinicaDental.Contrato` | Biblioteca compartida: DTOs que se envían a la API y modelos de lo que la API devuelve | — |

> El frontend **no accede a la base de datos**: todo pasa por la API. Por eso la API (y su SQL Server) deben estar en ejecución para usar el cliente.

Los 5 módulos están implementados: **Pacientes**, **Odontólogos (Especialidades y Horarios)**, **Citas**, **Tratamientos** y **Pagos**,
además de **inicio de sesión con JWT** y gestión de usuarios por rol.

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

4. Ejecuta el **frontend** (con la API ya corriendo), en otra terminal:

   ```bash
   dotnet run --project src/ClinicaDental.ClienteBlazor
   ```

   y abre `http://localhost:5180`. Ingresa con el usuario inicial **`admin`** / **`Admin.Sonrisa2026`** y cámbiale la contraseña en *Mi cuenta*.

**En Visual Studio (recomendado):** junto a la solución está `ClinicaDental.slnLaunch`. En la barra de herramientas elige el perfil **"API + Cliente web"** y pulsa ▶: arrancan los dos proyectos a la vez. (Alternativa: clic derecho en la solución → *Configurar proyectos de inicio* → *Varios proyectos de inicio* → `ClinicaDental.Web` y `ClinicaDental.ClienteBlazor` en *Iniciar*.)

Si cambias el puerto de la API, actualiza `ApiSettings:BaseUrl` en `src/ClinicaDental.ClienteBlazor/appsettings.json`.

## Frontend (ClinicaDental.ClienteBlazor) — Blazor

**Blazor Web App** con componentes `.razor` en modo **Interactive Server** (sin prerenderizado). Igual que el ejemplo de clase,
el cliente llama a la API con un `HttpClient` con nombre `"Api"` (configurado en `Program.cs`) y usa los DTOs del proyecto `Contrato`.

**Estilo visual:** inspirado en Material Design 3 (apps de Google): barra superior con buscador, menú lateral con
botón "Nueva cita", tarjetas, chips de estado, campos con etiqueta flotante, avisos tipo *snackbar* y diálogos de confirmación.
Paleta turquesa clínica (confianza e higiene) con azul sereno y menta. Incluye logotipo propio (diente sonriente),
**pantalla de carga** mientras arranca Blazor, **páginas de carga** (esqueletos animados) mientras llegan los datos,
**barra de progreso** durante cada llamada a la API, diseño adaptable a celular y comprobante de pago **imprimible**.
Íconos: *Material Symbols* y fuente *Roboto* (Google Fonts, requieren internet).

| Menú | Ruta | Endpoints que usa |
|---|---|---|
| Inicio de sesión | `/login` | `POST api/Auth/login` |
| Inicio | `/` (panel: citas de hoy, próximas citas, KPIs, saldos) | varios |
| Pacientes | `/pacientes`, `/pacientes/nuevo`, `/pacientes/{id}` (historial), `/pacientes/{id}/editar` | `api/Paciente` |
| Odontólogos | `/odontologos`, `/odontologos/nuevo`, `/odontologos/{id}` (horario semanal), `/odontologos/{id}/editar` | `api/Odontologo`, `api/HorarioOdontologo` |
| Especialidades | `/especialidades` | `api/Especialidad` |
| Citas | `/citas/agenda`, `/citas`, `/citas/nueva` (con horarios libres), `/citas/{id}`, `/citas/{id}/reprogramar`, `/citas/{id}/cancelar` | `api/Cita` |
| Tratamientos | `/tratamientos`, `/tratamientos/nuevo` (también desde una cita), `/tratamientos/{id}` (observaciones y pagos) | `api/Tratamiento` |
| Pagos | `/pagos`, `/pagos/nuevo`, `/pagos/deudas`, `/pagos/{id}/comprobante` | `api/Pago` |
| Mi cuenta | `/mi-cuenta` (cambiar contraseña) | `api/Auth/yo`, `api/Auth/contrasena` |
| Usuarios (solo Administrador) | `/usuarios` | `api/Auth/usuarios` |

Estructura del cliente:

```
src/ClinicaDental.ClienteBlazor/
├─ Program.cs                    HttpClient "Api", autenticación, servicios, cultura es-BO, cabeceras de seguridad
├─ appsettings.json              ApiSettings:BaseUrl → URL de la API
├─ Servicios/
│  ├─ ProveedorAutenticacion.cs  Sesión del usuario (token JWT) para <AuthorizeView>; se guarda cifrada en ProtectedSessionStorage
│  ├─ ClienteApi.cs              GET/POST/PUT/DELETE con el token; 401 → cierra sesión; convierte errores en mensajes
│  ├─ Notificaciones.cs          Snackbar y diálogo de confirmación
│  ├─ Formato.cs                 Textos, íconos y colores (estados, tipos, dinero, fechas)
│  └─ Disponibilidad.cs          Horarios libres para sugerir al agendar
├─ Components/
│  ├─ App.razor, Routes.razor    Documento HTML, pantalla de carga, enrutador
│  ├─ PaginaBase.cs              Clase base de todas las páginas (Api, Nav, Notificar, Dialogo, Cargando, errores)
│  ├─ Layout/                    MainLayout (exige sesión: barra, menú, cuenta) y LayoutAcceso (login)
│  ├─ Compartido/                Logo, Avatar, Encabezado, Cargando, Vacio, AvisoError, BotonEnviar, Snackbar,
│  │                             DialogoConfirmacion, ChipEstadoCita, ResumenCita, HorariosLibres, IndicadorContrasena…
│  └─ Pages/                     Login, Inicio, MiCuenta, Usuarios, Especialidades, Pacientes/, Odontologos/, Citas/, Tratamientos/, Pagos/
└─ wwwroot/app.css               Tema (paleta en variables CSS) · wwwroot/js/sonrisa.js (pantalla de carga, imprimir)
```

## Seguridad (inicio de sesión)

| Medida | Dónde |
|---|---|
| **Todos** los endpoints exigen token JWT, salvo `POST api/Auth/login` | API · `Program.cs` → `MapControllers().RequireAuthorization()` |
| Token **JWT firmado (HMAC-SHA256)** con emisor, audiencia y **expiración de 8 h** | API · `AuthServicio.GenerarToken`, sección `Jwt` de `appsettings.json` |
| Contraseñas con **PBKDF2-SHA256 + sal aleatoria (100 000 iteraciones)**; nunca en texto plano; comparación en tiempo constante | API · `Services/Seguridad/HasherContrasena.cs` |
| Política de **contraseña segura**: ≥ 8 caracteres con mayúscula, minúscula, número y símbolo | API (`ContrasenaSegura`) y formulario (`[ContrasenaSegura]` + indicador en vivo) |
| **Bloqueo** de la cuenta 15 min tras **5 intentos fallidos** | API · `AuthServicio.IniciarSesion` (HTTP 423) |
| **Límite de 10 intentos de login por minuto** por IP (fuerza bruta) | API · `AddRateLimiter` política `login` (HTTP 429) |
| Mensaje genérico "Usuario o contraseña incorrectos" (no revela si el usuario existe) | API |
| **Roles**: `Administrador` (gestiona usuarios), `Recepcionista`, `Odontologo` | API `[Authorize(Roles=...)]` · cliente `<AuthorizeView Roles=...>` |
| No se puede desactivar la propia cuenta ni dejar el sistema sin administradores | API · `AuthController` |
| En el cliente el token **no se expone a JavaScript**: vive en el servidor y se respalda cifrado (Data Protection) en `ProtectedSessionStorage`; se borra al cerrar la pestaña o la sesión | Cliente · `ProveedorAutenticacion` |
| Si el token expira o es inválido (401), el cliente cierra la sesión y vuelve al login | Cliente · `ClienteApi` |
| Toda página (salvo `/login`) exige sesión; el parámetro `?volver=` solo acepta rutas internas (evita redirecciones abiertas) | Cliente · `MainLayout`, `Login.razor` |
| Cabeceras `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` | Cliente · `Program.cs` |

> **Usuario inicial:** `admin` / `Admin.Sonrisa2026` (rol Administrador, creado por la migración `Usuarios`). Cámbiale la contraseña después del primer ingreso.
> **Clave JWT:** `Jwt:Clave` en `appsettings.json` sirve solo para desarrollo/clase. En producción debe venir de una variable de entorno (`Jwt__Clave`) o de *User Secrets*.
> En **Swagger** pulsa *Authorize* y pega el token que devuelve `POST /api/Auth/login` para probar los demás endpoints.
## Módulos y endpoints

Rutas base por controlador: `api/[controller]` (p. ej. `api/Paciente`, `api/Odontologo`, `api/Cita`). Endpoints tomados de los controladores.

### Autenticación y usuarios (`api/Auth`)

| Verbo | Ruta | Acceso | Name |
|---|---|---|---|
| POST | `/api/Auth/login` | Público | `IniciarSesion` |
| GET | `/api/Auth/yo` | Con sesión | `UsuarioActual` |
| PUT | `/api/Auth/contrasena` | Con sesión | `CambiarContrasena` |
| GET | `/api/Auth/usuarios` | Administrador | `ListarUsuarios` |
| POST | `/api/Auth/usuarios` | Administrador | `RegistrarUsuario` |
| PUT | `/api/Auth/usuarios/{id}/estado` | Administrador | `CambiarEstadoUsuario` |
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

Decisión del modelo: **Fecha y Hora van en propiedades separadas (DateOnly y TimeOnly)**.

Ejemplo `POST /api/Cita`:

```json
{
  "idPaciente": "30000000-0000-0000-0000-000000000001",
  "idOdontologo": "20000000-0000-0000-0000-000000000001",
  "fecha": "2026-12-01",
  "hora": "09:00:00",
  "duracionMinutos": 30,
  "motivo": "Limpieza"
}
```

Ejemplo `PUT /api/Cita/{id}/reprogramar`:

```json
{
  "nuevaFecha": "2026-12-02",
  "nuevaHora": "10:30:00"
}
```

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
| Usuario (1) | `70000000-0000-0000-0000-000000000001`: `admin` / `Admin.Sonrisa2026` (Administrador) |

No se siembran citas de ejemplo.

## Estructura de carpetas (vigente)

```
ClinicaDental.sln
ClinicaDental.slnLaunch  Perfil de Visual Studio para arrancar API + cliente juntos
├─ src/ClinicaDental.ClienteBlazor/   Frontend (Blazor Web App), ver sección "Frontend"
├─ src/ClinicaDental.Contrato/        DTOs/ (lo que se envía a la API), Respuestas/ (lo que devuelve), Enums.cs
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
Navegador ──► ClinicaDental.ClienteBlazor (Blazor Interactive Server, puerto 5180)
                 │  HttpClient "Api" + token JWT (Authorization: Bearer) + DTOs de ClinicaDental.Contrato
                 ▼
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
