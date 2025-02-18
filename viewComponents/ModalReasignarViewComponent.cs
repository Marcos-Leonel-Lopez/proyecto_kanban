using Microsoft.AspNetCore.Mvc;
public class ModalReasignarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int idTarea, List<UsuarioViewModel> usuarios, int idPropietario)
    {
        var mensaje = new ModalReasignarViewModel
        {
            IdTarea = idTarea,
            Usuarios = usuarios,
            IdPropietario = idPropietario
        };
        return View(mensaje);
    }
}

