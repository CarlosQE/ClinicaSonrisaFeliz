using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.DTOs.Tratamiento
{
    /// <summary>Cuerpo de PUT /api/Tratamiento/{id}/observaciones.</summary>
    public class ActualizarObservacionesDto : IValidatableObject
    {
        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Las observaciones deben tener entre 3 y 1000 caracteres.")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Observaciones))
                yield break;

            if (TratamientoValidaciones.TieneCaracteresProhibidos(Observaciones))
            {
                yield return new ValidationResult(
                    "Las observaciones no pueden contener '<', '>' ni caracteres de control.",
                    [nameof(Observaciones)]);
                yield break;
            }

            var normalizado = TratamientoValidaciones.NormalizarTexto(Observaciones);
            if (normalizado.Length < TratamientoValidaciones.ObservacionesMinimo || normalizado.Length > TratamientoValidaciones.ObservacionesMaximo)
            {
                yield return new ValidationResult(
                    $"Las observaciones deben tener entre {TratamientoValidaciones.ObservacionesMinimo} y {TratamientoValidaciones.ObservacionesMaximo} caracteres.",
                    [nameof(Observaciones)]);
            }
        }
    }
}
