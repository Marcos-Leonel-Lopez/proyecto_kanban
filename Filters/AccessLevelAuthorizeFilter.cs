using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AccessLevelAuthorizeFilter : IAuthorizationFilter
{
    private readonly string[] _requiredAccessLevels;
    private readonly ILogger<AccessLevelAuthorizeFilter> _logger;

    public AccessLevelAuthorizeFilter(ILogger<AccessLevelAuthorizeFilter> logger, string[] requiredAccessLevels)
    {
        _logger = logger;
        _requiredAccessLevels = requiredAccessLevels ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userAccessLevel = context.HttpContext.Session.GetString("AccessLevel");
        
        string nombreUsuario = context.HttpContext.Session.GetString("nombreUsuario") ?? "Desconocido";

        if (!IsAuthenticated(context))
        {
            _logger.LogWarning($"Acceso NO autorizado: Usuario {nombreUsuario} intentó acceder a {context.ActionDescriptor.DisplayName} sin estar autenticado.");
            context.HttpContext.Response.StatusCode = 401;
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;

        }

        if (_requiredAccessLevels.Length > 0 && !_requiredAccessLevels.Contains(userAccessLevel))
        {
            int idUsuario = context.HttpContext.Session.GetInt32("idUsuario").Value;
            _logger.LogWarning($"Acceso DENEGADO: Usuario {nombreUsuario}-#{idUsuario}   intentó acceder a {context.ActionDescriptor.DisplayName} sin los permisos adecuados.");
            context.Result = new ViewResult
            {
                ViewName = "Forbidden",
                StatusCode = 403
            };
            return;
        }

        //_logger.LogInformation($"Acceso permitido: Usuario {nombreUsuario} accedió a {context.ActionDescriptor.DisplayName}.");
    }

    private static bool IsAuthenticated(AuthorizationFilterContext context) => context.HttpContext.Session.GetString("IsAuthenticated") == "true";
}
