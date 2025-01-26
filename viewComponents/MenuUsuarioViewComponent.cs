using Microsoft.AspNetCore.Mvc;
public class MenuUsuarioViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string rolUsuario)
    {
        // Selecciona la vista según el rol del usuario.
        return rolUsuario switch
        {
            var rol when rol == MisEnums.RolUsuario.Administrador.ToString() => View("Administrador"),
            var rol when rol == MisEnums.RolUsuario.Operador.ToString() => View("Operador"),
            _ => View("Operador") // no tengo una vista default
        };

    }
}