using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClinicaDental.Contrato.DTOs;
using Microsoft.AspNetCore.Components;

namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>
    /// Único punto de contacto con la API (HttpClient con nombre "Api", ver Program.cs).
    /// - Agrega el token JWT de la sesión a cada solicitud (encabezado Authorization: Bearer).
    /// - Si la API responde 401, la sesión expiró: se cierra y se envía al usuario al login.
    /// - Convierte cualquier respuesta (200, 400, 404, 423, 429, 500, sin conexión) en un <see cref="ResultadoApi"/>
    ///   con un mensaje en español listo para mostrar.
    /// - Avisa cuántas solicitudes hay en curso para mostrar la barra de progreso superior.
    /// </summary>
    public class ClienteApi
    {
        public const string MensajeSinConexion =
            "No se pudo conectar con la API de la clínica. Verifica que el proyecto ClinicaDental.Web esté en ejecución.";

        private readonly HttpClient _http;
        private readonly ProveedorAutenticacion _auth;
        private readonly NavigationManager _nav;
        private int _enCurso;

        public ClienteApi(IHttpClientFactory httpClientFactory, ProveedorAutenticacion auth, NavigationManager nav)
        {
            _http = httpClientFactory.CreateClient("Api");
            _auth = auth;
            _nav = nav;
        }

        /// <summary>Se dispara cuando empieza o termina una solicitud (para la barra de progreso).</summary>
        public event Action? CargaCambio;
        public bool Cargando => _enCurso > 0;

        // ---------------- Autenticación ----------------

        public Task<ResultadoApi<LoginRespuestaDto>> IniciarSesionAsync(LoginDto dto)
            => EnviarAsync<LoginRespuestaDto>(HttpMethod.Post, "api/Auth/login", dto, conToken: false);

        // ---------------- Verbos ----------------

        public Task<ResultadoApi<T>> GetAsync<T>(string url) => EnviarAsync<T>(HttpMethod.Get, url, null);
        public Task<ResultadoApi<T>> PostAsync<T>(string url, object cuerpo) => EnviarAsync<T>(HttpMethod.Post, url, cuerpo);
        public Task<ResultadoApi<T>> PutAsync<T>(string url, object cuerpo) => EnviarAsync<T>(HttpMethod.Put, url, cuerpo);

        /// <summary>Para endpoints que responden texto ("Paciente eliminado correctamente.").</summary>
        public Task<ResultadoApi<string>> PutTextoAsync(string url, object cuerpo) => EnviarAsync<string>(HttpMethod.Put, url, cuerpo);
        public Task<ResultadoApi<string>> DeleteAsync(string url) => EnviarAsync<string>(HttpMethod.Delete, url, null);

        private async Task<ResultadoApi<T>> EnviarAsync<T>(HttpMethod metodo, string url, object? cuerpo, bool conToken = true)
        {
            CambiarCarga(+1);
            try
            {
                using var solicitud = new HttpRequestMessage(metodo, url);
                if (cuerpo is not null) solicitud.Content = JsonContent.Create(cuerpo, cuerpo.GetType());
                if (conToken && _auth.Token is { } token)
                {
                    solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using var respuesta = await _http.SendAsync(solicitud);

                if (conToken && respuesta.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await _auth.CerrarSesionAsync();
                    _nav.NavigateTo("login?expirada=1");
                    return new ResultadoApi<T> { Estado = respuesta.StatusCode, Mensaje = "Tu sesión expiró. Inicia sesión nuevamente." };
                }

                if (respuesta.StatusCode == HttpStatusCode.Forbidden && conToken)
                {
                    return new ResultadoApi<T> { Estado = respuesta.StatusCode, Mensaje = "No tienes permiso para realizar esta acción." };
                }

                return await LeerAsync<T>(respuesta, "La API no pudo completar la operación.");
            }
            catch (HttpRequestException) { return new ResultadoApi<T> { SinConexion = true, Mensaje = MensajeSinConexion }; }
            catch (TaskCanceledException) { return new ResultadoApi<T> { SinConexion = true, Mensaje = MensajeSinConexion }; }
            finally
            {
                CambiarCarga(-1);
            }
        }

        private void CambiarCarga(int delta)
        {
            Interlocked.Add(ref _enCurso, delta);
            CargaCambio?.Invoke();
        }

        private static async Task<ResultadoApi<T>> LeerAsync<T>(HttpResponseMessage respuesta, string mensajePorDefecto)
        {
            if (respuesta.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                {
                    var texto = LimpiarTexto(await respuesta.Content.ReadAsStringAsync());
                    return new ResultadoApi<T> { Exito = true, Estado = respuesta.StatusCode, Datos = (T)(object)texto, Mensaje = texto };
                }

                var datos = respuesta.Content.Headers.ContentLength == 0 ? default : await respuesta.Content.ReadFromJsonAsync<T>();
                return new ResultadoApi<T> { Exito = true, Estado = respuesta.StatusCode, Datos = datos };
            }

            return new ResultadoApi<T>
            {
                Estado = respuesta.StatusCode,
                Mensaje = await ObtenerMensajeErrorAsync(respuesta, mensajePorDefecto)
            };
        }

        /// <summary>
        /// La API responde errores como texto plano ("El CI ya está registrado.") o como
        /// ValidationProblemDetails ({"errors": {"Campo": ["mensaje"]}}). Se unifican en un solo texto.
        /// </summary>
        private static async Task<string> ObtenerMensajeErrorAsync(HttpResponseMessage respuesta, string mensajePorDefecto)
        {
            var contenido = await respuesta.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contenido)) return mensajePorDefecto;

            var tipo = respuesta.Content.Headers.ContentType?.MediaType ?? string.Empty;
            if (!tipo.Contains("json", StringComparison.OrdinalIgnoreCase)) return LimpiarTexto(contenido);

            try
            {
                using var doc = JsonDocument.Parse(contenido);
                var raiz = doc.RootElement;
                if (raiz.ValueKind == JsonValueKind.String) return raiz.GetString() ?? mensajePorDefecto;

                if (raiz.ValueKind == JsonValueKind.Object)
                {
                    if (raiz.TryGetProperty("errors", out var errores) && errores.ValueKind == JsonValueKind.Object)
                    {
                        var mensajes = errores.EnumerateObject()
                            .SelectMany(p => p.Value.ValueKind == JsonValueKind.Array ? p.Value.EnumerateArray() : Enumerable.Empty<JsonElement>())
                            .Select(e => e.GetString())
                            .Where(m => !string.IsNullOrWhiteSpace(m))
                            .Distinct()
                            .ToList();
                        if (mensajes.Count > 0) return string.Join(" ", mensajes);
                    }
                    if (raiz.TryGetProperty("title", out var titulo) && titulo.GetString() is { Length: > 0 } t) return t;
                }
            }
            catch (JsonException)
            {
                return LimpiarTexto(contenido);
            }
            return mensajePorDefecto;
        }

        private static string LimpiarTexto(string texto) => texto.Trim().Trim('"');
    }
}
