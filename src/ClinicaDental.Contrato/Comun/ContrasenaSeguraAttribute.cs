using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Contrato.Comun
{
    /// <summary>
    /// Misma política que la API: mínimo 8 caracteres con mayúscula, minúscula, número y símbolo.
    /// Se valida en el formulario para avisar antes de enviar; la API vuelve a validar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ContrasenaSeguraAttribute : ValidationAttribute
    {
        public ContrasenaSeguraAttribute()
            : base("La contraseña debe tener al menos 8 caracteres, con mayúscula, minúscula, número y símbolo.") { }

        public override bool IsValid(object? value)
        {
            if (value is not string c || c.Length == 0) return true; // de vacío se encarga [Required]
            return c.Length is >= 8 and <= 100
                && c.Any(char.IsUpper)
                && c.Any(char.IsLower)
                && c.Any(char.IsDigit)
                && c.Any(x => !char.IsLetterOrDigit(x));
        }
    }
}
