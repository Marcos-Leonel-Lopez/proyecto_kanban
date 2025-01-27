using Microsoft.AspNetCore.Mvc;

namespace trabajo_final.Components.Tarea.Cabecera;
public class CabeceraViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool propietario, int? id_usuario_asignado, List<UsuarioViewModel> usuarios)
    {
        var cabecera = new CabeceraViewModel
        {
            EsPropietario = propietario,
            IdUsuarioAsignado = id_usuario_asignado,
            Usuarios = usuarios
        };
        return View(cabecera);
    }
}
