using System.ComponentModel.DataAnnotations;

namespace ClinicaDental.Web.DTOs.Odontologo
{
    public class CambiarEstadoOdontologoDto
    {
        // Se usa bool? para que, si el cliente no envía el campo,
        // el controlador pueda responder con un error en lugar de asumir "false".
        [Required(ErrorMessage = "Debe indicar si el odontólogo queda activo o inactivo.")]
        public bool? Activo { get; set; }
    }
}
