using Microsoft.AspNetCore.Mvc;

public class MenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool loggeado, string rolUsuario)
    {
        // Datos necesarios a la vista.
        var model = new MenuViewModel
        {
            Loggeado = loggeado,
            RolUsuario = rolUsuario
        };
        return View(model);
    }
}


