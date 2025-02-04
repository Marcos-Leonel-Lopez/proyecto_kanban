using Microsoft.AspNetCore.Mvc;

public class MenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool loggeado, string rolUsuario)
    {
        if (!loggeado)
        {
            // Si no está logueado, devuelve la vista general.
            return View("Default");
        }

        // Selecciona la vista según el rol del usuario.
        return rolUsuario switch
        {
            var rol when rol == MisEnums.RolUsuario.Administrador.ToString() => View("Administrador"),
            var rol when rol == MisEnums.RolUsuario.Operador.ToString() => View("Operador"),
            _ => View("Default")
        };

    }
}


