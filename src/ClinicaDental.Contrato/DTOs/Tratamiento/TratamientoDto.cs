using System.ComponentModel.DataAnnotations;
using ClinicaDental.Contrato.Enums;

namespace ClinicaDental.Contrato.DTOs.Tratamiento
{
    /// <summary>Cuerpo de POST /api/Tratamiento.</summary>
    public class TratamientoDto : IValidatableObject
    {
        [Required(ErrorMessage = "El paciente es obligatorio.")]
        [Display(Name = "Paciente")]
        public Guid? IdPaciente { get; set; }

        [Required(ErrorMessage = "El odontólogo es obligatorio.")]
        [Display(Name = "Odontólogo")]
        public Guid? IdOdontologo { get; set; }

        [Display(Name = "Cita asociada (opcional)")]
        public Guid? IdCita { get; set; }

        [Required(ErrorMessage = "El tipo de tratamiento es obligatorio.")]
        [EnumDataType(typeof(TipoTratamiento), ErrorMessage = "El tipo de tratamiento no es válido (0 = LimpiezaDental, 1 = Ortodoncia, 2 = Endodoncia, 3 = Extraccion).")]
        [Display(Name = "Tipo de tratamiento")]
        public TipoTratamiento Tipo { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateOnly? Fecha { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "El costo debe estar entre 0.01 y 1,000,000 Bs.")]
        [Display(Name = "Costo (Bs)")]
        public decimal Costo { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IdPaciente == Guid.Empty)
                yield return new ValidationResult("El paciente es obligatorio.", [nameof(IdPaciente)]);

            if (IdOdontologo == Guid.Empty)
                yield return new ValidationResult("El odontólogo es obligatorio.", [nameof(IdOdontologo)]);

            if (IdCita == Guid.Empty)
                yield return new ValidationResult("El id de la cita no es válido.", [nameof(IdCita)]);

            if (!Enum.IsDefined(Tipo))
                yield return new ValidationResult("El tipo de tratamiento no es válido.", [nameof(Tipo)]);

            if (Fecha.HasValue)
            {
                if (Fecha.Value == default || Fecha.Value < TratamientoValidaciones.FechaMinima || Fecha.Value > DateOnly.FromDateTime(DateTime.Today))
                {
                    yield return new ValidationResult(
                        "La fecha del tratamiento no puede ser futura ni anterior a 2000-01-01.",
                        [nameof(Fecha)]);
                }
            }

            if (decimal.Round(Costo, 2) != Costo)
                yield return new ValidationResult("El costo puede tener como máximo 2 decimales.", [nameof(Costo)]);

            if (!string.IsNullOrWhiteSpace(Observaciones))
            {
                if (TratamientoValidaciones.TieneCaracteresProhibidos(Observaciones))
                {
                    yield return new ValidationResult(
                        "Las observaciones no pueden contener '<', '>' ni caracteres de control.",
                        [nameof(Observaciones)]);
                }
                else
                {
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
    }
}
