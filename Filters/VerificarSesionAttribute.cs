using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class VerificarSesionAttribute : ActionFilterAttribute
{
    public VerificarSesionAttribute()
    {
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var idUsuario = httpContext.Session.GetString("idUsuario");

        if (string.IsNullOrEmpty(idUsuario))
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}


