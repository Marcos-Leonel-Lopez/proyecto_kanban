using Microsoft.AspNetCore.Mvc;
public class TareaCabeceraViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(bool propietario, int? idUsuarioAsignado, List<UsuarioViewModel> usuarios, int idTarea)
    {
        var cabecera = new TareaCabeceraViewModel
        {
            EsPropietario = propietario,
            IdUsuarioAsignado = idUsuarioAsignado,
            Usuarios = usuarios,
            IdTarea = idTarea
        };
        return View(cabecera);
    }
}