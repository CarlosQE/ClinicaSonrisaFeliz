using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.Enums
{
    public enum TipoTratamiento
    {
        [Display(Name = "Limpieza dental")] LimpiezaDental = 0,
        [Display(Name = "Ortodoncia")] Ortodoncia = 1,
        [Display(Name = "Endodoncia")] Endodoncia = 2,
        [Display(Name = "Extracción")] Extraccion = 3
    }
}
