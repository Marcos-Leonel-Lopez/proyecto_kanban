using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class VerificarSesionAttribute : ActionFilterAttribute
{
    private readonly bool _requireAdmin;

    public VerificarSesionAttribute(bool requireAdmin = false)
    {
        _requireAdmin = requireAdmin;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var idUsuario = httpContext.Session.GetString("idUsuario");
        var rolUsuario = httpContext.Session.GetString("rolUsuario");

        if (string.IsNullOrEmpty(idUsuario))
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        if (_requireAdmin && rolUsuario != MisEnums.RolUsuario.Administrador.ToString())
        {
            context.Result = new ViewResult
            {
                ViewName = "Forbidden",
                StatusCode = 403
            };
            return;
        }

        base.OnActionExecuting(context);
    }
}
