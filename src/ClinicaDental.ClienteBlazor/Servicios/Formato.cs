using System.Globalization;
using ClinicaDental.Contrato.Enums;

namespace ClinicaDental.ClienteBlazor.Servicios
{
    /// <summary>Textos, colores e íconos comunes para mostrar datos de la API de forma consistente.</summary>
    public static class Formato
    {
        private static readonly CultureInfo Es = new("es-BO");

        public static string Dinero(decimal monto) => "Bs " + monto.ToString("#,##0.00", CultureInfo.InvariantCulture);

        public static string Fecha(DateOnly fecha) => fecha.ToString("dd/MM/yyyy");
        public static string Fecha(DateTime fecha) => fecha.ToString("dd/MM/yyyy");
        public static string FechaHora(DateOnly fecha, TimeOnly hora) => $"{fecha:dd/MM/yyyy} {hora:HH:mm}";
        public static string FechaHora(DateTime fecha) => fecha.ToString("dd/MM/yyyy HH:mm");
        public static string Hora(DateTime fecha) => fecha.ToString("HH:mm");
        public static string Hora(TimeOnly hora) => hora.ToString("HH:mm");

        /// <summary>"lunes, 24 de noviembre de 2026"</summary>
        public static string FechaLarga(DateOnly fecha) => fecha.ToString("dddd, d 'de' MMMM 'de' yyyy", Es);
        public static string FechaLarga(DateTime fecha) => FechaLarga(DateOnly.FromDateTime(fecha));

        public static string Dia(DayOfWeek dia) => Es.DateTimeFormat.GetDayName(dia) switch
        {
            var n => char.ToUpper(n[0]) + n[1..]
        };

        public static int Edad(DateOnly nacimiento, DateOnly hoy)
        {
            var edad = hoy.Year - nacimiento.Year;
            if (hoy < nacimiento.AddYears(edad)) edad--;
            return edad;
        }

        /// <summary>Texto en minúsculas y sin tildes para el filtro instantáneo (atributo data-texto).</summary>
        public static string TextoBusqueda(params string?[] partes)
        {
            var texto = string.Join(" ", partes.Where(p => !string.IsNullOrWhiteSpace(p))).ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
            return new string(texto.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        }

        /// <summary>Iniciales a partir de un nombre completo ("Ana Rojas" → "AR").</summary>
        public static string IniciasDe(string? nombreCompleto)
        {
            var partes = (nombreCompleto ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length switch
            {
                0 => "?",
                1 => partes[0][..1].ToUpperInvariant(),
                _ => (partes[0][..1] + partes[^1][..1]).ToUpperInvariant()
            };
        }

        public static string Iniciales(string nombres, string apellidos)
            => $"{Primera(nombres)}{Primera(apellidos)}".ToUpperInvariant();

        private static string Primera(string texto) => string.IsNullOrWhiteSpace(texto) ? "" : texto.Trim()[..1];

        /// <summary>Color de avatar estable a partir de un texto (como en Gmail).</summary>
        public static string ColorAvatar(string semilla)
        {
            string[] colores = ["#00696D", "#3B6470", "#4B607C", "#6B5778", "#8C4A60", "#7A5900", "#2E6A44", "#4E5B92"];
            var suma = semilla.Aggregate(0, (acc, c) => acc + c);
            return colores[suma % colores.Length];
        }

        // ---------------- Estados de cita ----------------
        public static string Texto(EstadoCita e) => e switch
        {
            EstadoCita.Programada => "Programada",
            EstadoCita.Reprogramada => "Reprogramada",
            EstadoCita.Cancelada => "Cancelada",
            EstadoCita.Atendida => "Atendida",
            _ => e.ToString()
        };

        public static string ClaseChip(EstadoCita e) => e switch
        {
            EstadoCita.Programada => "chip--primary",
            EstadoCita.Reprogramada => "chip--warn",
            EstadoCita.Cancelada => "chip--neutral",
            EstadoCita.Atendida => "chip--success",
            _ => "chip--neutral"
        };

        public static string Icono(EstadoCita e) => e switch
        {
            EstadoCita.Programada => "event_available",
            EstadoCita.Reprogramada => "update",
            EstadoCita.Cancelada => "event_busy",
            EstadoCita.Atendida => "task_alt",
            _ => "event"
        };

        // ---------------- Tratamientos ----------------
        public static string Texto(TipoTratamiento t) => t switch
        {
            TipoTratamiento.LimpiezaDental => "Limpieza dental",
            TipoTratamiento.Ortodoncia => "Ortodoncia",
            TipoTratamiento.Endodoncia => "Endodoncia",
            TipoTratamiento.Extraccion => "Extracción",
            _ => t.ToString()
        };

        public static string Icono(TipoTratamiento t) => t switch
        {
            TipoTratamiento.LimpiezaDental => "clean_hands",
            TipoTratamiento.Ortodoncia => "linear_scale",
            TipoTratamiento.Endodoncia => "healing",
            TipoTratamiento.Extraccion => "dentistry",
            _ => "medical_services"
        };

        /// <summary>La API de deudas/comprobantes envía el tipo como texto ("LimpiezaDental").</summary>
        public static string TextoTipo(string tipo)
            => Enum.TryParse<TipoTratamiento>(tipo, out var t) ? Texto(t) : tipo;

        // ---------------- Pagos ----------------
        public static string Texto(MetodoPago m) => m switch
        {
            MetodoPago.Efectivo => "Efectivo",
            MetodoPago.Tarjeta => "Tarjeta",
            MetodoPago.Transferencia => "Transferencia",
            MetodoPago.QR => "QR",
            _ => m.ToString()
        };

        public static string Icono(MetodoPago m) => m switch
        {
            MetodoPago.Efectivo => "payments",
            MetodoPago.Tarjeta => "credit_card",
            MetodoPago.Transferencia => "account_balance",
            MetodoPago.QR => "qr_code_2",
            _ => "payments"
        };

        public static string TextoMetodo(string metodo)
            => Enum.TryParse<MetodoPago>(metodo, out var m) ? Texto(m) : metodo;
    }
}
