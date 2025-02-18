using Microsoft.AspNetCore.Mvc;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        bool isLoggedIn = HttpContext.Session.GetInt32("idUsuario") != null;
        bool isAdmin = HttpContext.Session.GetString("rolUsuario") == MisEnums.RolUsuario.Administrador.ToString();
        return View(isLoggedIn);
    }
}
