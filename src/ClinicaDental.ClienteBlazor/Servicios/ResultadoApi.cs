using System.Net;

namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>Resultado de una llamada a la API: éxito + datos, o un mensaje de error listo para mostrar.</summary>
    public class ResultadoApi
    {
        public bool Exito { get; init; }
        public string Mensaje { get; init; } = string.Empty;
        public HttpStatusCode? Estado { get; init; }

        /// <summary>true si la API no respondió (apagada, URL incorrecta, etc.).</summary>
        public bool SinConexion { get; init; }

        public bool NoEncontrado => Estado == HttpStatusCode.NotFound;
    }

    public class ResultadoApi<T> : ResultadoApi
    {
        public T? Datos { get; init; }
    }
}
