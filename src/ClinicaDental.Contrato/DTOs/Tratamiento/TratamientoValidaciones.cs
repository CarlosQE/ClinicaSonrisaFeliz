using System.Text.RegularExpressions;

namespace ClinicaDental.Contrato.DTOs.Tratamiento
{
    /// <summary>Reglas compartidas de validación y normalización para tratamientos.</summary>
    public static class TratamientoValidaciones
    {
        public const decimal CostoMinimo = 0.01m;
        public const decimal CostoMaximo = 1_000_000m;
        public const int ObservacionesMinimo = 3;
        public const int ObservacionesMaximo = 1000;
        public static readonly DateOnly FechaMinima = new(2000, 1, 1);

        /// <summary>Recorta extremos, normaliza saltos de línea y colapsa espacios múltiples internos.</summary>
        public static string NormalizarTexto(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            var sinRetorno = texto.Trim().Replace("\r\n", "\n").Replace('\r', '\n');
            var lineas = sinRetorno.Split('\n')
                .Select(l => Regex.Replace(l.Trim(), @" +", " "));
            return string.Join('\n', lineas).Trim();
        }

        /// <summary>Rechaza etiquetas HTML (&lt;, &gt;) y caracteres de control distintos de salto de línea.</summary>
        public static bool TieneCaracteresProhibidos(string texto)
        {
            return texto.Contains('<')
                || texto.Contains('>')
                || texto.Any(c => char.IsControl(c) && c != '\n' && c != '\r');
        }

        /// <summary>Costo entre 0.01 y 1,000,000 Bs con hasta 2 decimales.</summary>
        public static bool EsCostoValido(decimal costo)
        {
            return costo >= CostoMinimo
                && costo <= CostoMaximo
                && decimal.Round(costo, 2) == costo;
        }
    }
}
