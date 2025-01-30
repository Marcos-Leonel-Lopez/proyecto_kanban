using Microsoft.AspNetCore.Mvc;
public class TareaCabeceraViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool propietario, int? id_usuario_asignado, List<UsuarioViewModel> usuarios)
    {
        var cabecera = new TareaCabeceraViewModel
        {
            EsPropietario = propietario,
            IdUsuarioAsignado = id_usuario_asignado,
            Usuarios = usuarios
        };
        return View(cabecera);
    }
}