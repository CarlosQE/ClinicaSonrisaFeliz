using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ClinicaDental.Web.Common;

/// <summary>
/// Filtro global (registrado en Program.cs). Traduce excepciones "esperadas" a respuestas HTTP:
///  - NoEncontradoException   -> 404 Not Found
///  - NotImplementedException -> 501 con la vista "EnConstruccion" (módulo aún no implementado)
/// ReglaNegocioException NO se maneja aquí: cada controlador la captura y la muestra en el formulario.
/// </summary>
public class FiltroExcepcionesDominio : IExceptionFilter
{
    private readonly IModelMetadataProvider _metadata;

    public FiltroExcepcionesDominio(IModelMetadataProvider metadata) => _metadata = metadata;

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NoEncontradoException:
                context.Result = new NotFoundResult();
                context.ExceptionHandled = true;
                break;

            case NotImplementedException ex:
                context.Result = new ViewResult
                {
                    ViewName = "EnConstruccion",
                    StatusCode = StatusCodes.Status501NotImplemented,
                    ViewData = new ViewDataDictionary(_metadata, context.ModelState) { ["Mensaje"] = ex.Message }
                };
                context.ExceptionHandled = true;
                break;
        }
    }
}
