using Microsoft.AspNetCore.Mvc;
public class MenuTareaViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string rolUsuario)
    {
        int idUsuario = (int)HttpContext.Session.GetInt32("idUsuario").Value;
        return rolUsuario switch
        {
            var rol when rol == MisEnums.RolUsuario.Administrador.ToString() => View("Administrador",idUsuario),
            var rol when rol == MisEnums.RolUsuario.Operador.ToString() => View("Operador",idUsuario),
            _ => View("Operador") // no tengo una vista default
        };

    }
}