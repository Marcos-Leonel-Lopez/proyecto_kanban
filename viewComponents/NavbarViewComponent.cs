using Microsoft.AspNetCore.Mvc;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        bool isLoggedIn = HttpContext.Session.GetInt32("idUsuario") != null;
        return View(isLoggedIn);
    }
}
