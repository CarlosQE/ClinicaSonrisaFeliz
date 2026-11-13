namespace ClinicaDental.Web.Common;

// ==========================================================================
//  CONTRATO COMPARTIDO. Todos los servicios usan SOLO estas dos excepciones
//  para comunicar errores "esperados" al controlador:
//
//   - NoEncontradoException   -> el id recibido no existe   (controlador: NotFound())
//   - ReglaNegocioException   -> se violó una regla          (controlador: ModelState.AddModelError)
//
//  Los tests verifican estas excepciones, así que NO lances otras
//  (InvalidOperationException, ArgumentException, etc.) para estos casos.
// ==========================================================================

/// <summary>Se lanza cuando se viola una regla de negocio (CI duplicado, horario ocupado, etc.).</summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}

/// <summary>Se lanza cuando una operación recibe el id de un registro que no existe.</summary>
public class NoEncontradoException : Exception
{
    public NoEncontradoException(string entidad, int id)
        : base($"{entidad} con id {id} no existe.") { }
}
