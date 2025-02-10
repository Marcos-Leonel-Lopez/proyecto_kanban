using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;


public class ExceptionHandlerService
{
    private readonly ILogger<ExceptionHandlerService> _logger;

    public ExceptionHandlerService(ILogger<ExceptionHandlerService> logger)
    {
        _logger = logger;
    }

    public IActionResult HandleException(Exception ex, string controlador, string accion)
    {
        int statusCode;
        string errorMessage;
        string view = "Error";

        switch (ex)
        {
            case NoEncontradoException notFoundEx:
                _logger.LogWarning(notFoundEx.Message);
                statusCode = 404;
                errorMessage = notFoundEx.Message;
                break;

            case SqliteException dbEx:
                _logger.LogError($"Error en base de datos en {controlador}/{accion}: {dbEx.Message}");
                statusCode = 500;
                errorMessage = "Hubo un problema con la base de datos. Intente más tarde.";
                break;
            case NoAutorizadoException authEx:
                _logger.LogWarning(authEx.Message);
                statusCode = 403;
                errorMessage = authEx.Message;
                view = "Forbidden";
                break;
            case AccesoDenegadoException accessEx:
                _logger.LogWarning(accessEx.Message);
                statusCode = 401;
                errorMessage = accessEx.Message;
                view = "Unauthorized";
                break;
            default:
                _logger.LogError($"Error inesperado en {controlador}/{accion}: {ex.Message}");
                statusCode = 500;
                errorMessage = "Hubo un problema inesperado. Intente nuevamente.";
                break;
        }

        var errorViewModel = new MyErrorViewModel
        {
            ErrorMessage = errorMessage,
            StatusCode = statusCode,
            Controller = controlador
        };

        return new ViewResult
        {
            ViewName = view,
            ViewData = new ViewDataDictionary(
        new EmptyModelMetadataProvider(),
        new ModelStateDictionary())
            {
                Model = errorViewModel
            },
            StatusCode = statusCode
        };
    }
}
