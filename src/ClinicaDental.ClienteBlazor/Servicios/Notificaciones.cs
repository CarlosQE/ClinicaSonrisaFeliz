namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>Muestra mensajes tipo "snackbar" (abajo a la izquierda). Uso: Notificaciones.Exito("Guardado").</summary>
    public class Notificaciones
    {
        public event Action<string, bool>? AlMostrar;

        public void Exito(string mensaje) => AlMostrar?.Invoke(mensaje, false);
        public void Error(string mensaje) => AlMostrar?.Invoke(mensaje, true);
    }

    /// <summary>Diálogo de confirmación modal. Uso: if (await Dialogo.ConfirmarAsync("¿Eliminar?", "...")) { ... }</summary>
    public class ServicioDialogo
    {
        private TaskCompletionSource<bool>? _respuesta;

        public event Action<string, string, string>? AlAbrir;

        public Task<bool> ConfirmarAsync(string titulo, string mensaje, string textoBoton = "Eliminar")
        {
            _respuesta?.TrySetResult(false);
            _respuesta = new TaskCompletionSource<bool>();
            AlAbrir?.Invoke(titulo, mensaje, textoBoton);
            return _respuesta.Task;
        }

        public void Responder(bool aceptar) => _respuesta?.TrySetResult(aceptar);
    }
}
