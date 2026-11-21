using ClinicaDental.ClienteBlazor.Servicios;
using ClinicaDental.Contrato.Respuestas;
using Microsoft.AspNetCore.Components;

namespace ClinicaDental.ClienteBlazor.Components
{
    /// <summary>
    /// Clase base de TODAS las páginas que consumen la API (@inherits PaginaBase, ya puesto en Pages/_Imports.razor).
    /// Da acceso a la API, navegación, notificaciones, diálogo de confirmación y helpers de carga.
    /// </summary>
    public abstract class PaginaBase : ComponentBase
    {
        [Inject] protected ClienteApi Api { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected Notificaciones Notificar { get; set; } = default!;
        [Inject] protected ServicioDialogo Dialogo { get; set; } = default!;
        [Inject] protected TimeProvider Reloj { get; set; } = default!;

        protected DateTime Ahora => Reloj.GetLocalNow().DateTime;
        protected DateOnly Hoy => DateOnly.FromDateTime(Ahora);

        /// <summary>true mientras se cargan los datos iniciales (muestra el componente &lt;Cargando /&gt;).</summary>
        protected bool Cargando { get; set; } = true;

        /// <summary>true mientras se envía un formulario (deshabilita el botón y muestra el spinner).</summary>
        protected bool Enviando { get; set; }

        /// <summary>Error al cargar datos (API apagada, 404, etc.).</summary>
        protected string? MensajeError { get; set; }
        protected bool SinConexion { get; set; }
        protected bool NoEncontrado { get; set; }

        /// <summary>Error devuelto por la API al guardar un formulario.</summary>
        protected string? ErrorFormulario { get; set; }

        /// <summary>GET que, si falla, registra el error y devuelve un valor por defecto.</summary>
        protected async Task<T> CargarAsync<T>(string url, T porDefecto)
        {
            var r = await Api.GetAsync<T>(url);
            if (r.Exito && r.Datos is not null) return r.Datos;
            RegistrarError(r);
            return porDefecto;
        }

        protected void RegistrarError(ResultadoApi r)
        {
            SinConexion |= r.SinConexion;
            NoEncontrado |= r.NoEncontrado;
            MensajeError ??= r.Mensaje;
        }

        protected void LimpiarErrores()
        {
            MensajeError = null;
            SinConexion = false;
            NoEncontrado = false;
            ErrorFormulario = null;
        }

        // ---------------- Opciones para listas desplegables ----------------

        protected Task<List<PacienteRespuesta>> PacientesAsync() => CargarAsync("api/Paciente", new List<PacienteRespuesta>());

        protected Task<List<OdontologoRespuesta>> OdontologosAsync(bool soloActivos = true)
            => CargarAsync(soloActivos ? "api/Odontologo?activo=true" : "api/Odontologo", new List<OdontologoRespuesta>());

        protected Task<List<EspecialidadRespuesta>> EspecialidadesAsync() => CargarAsync("api/Especialidad", new List<EspecialidadRespuesta>());
    }
}
