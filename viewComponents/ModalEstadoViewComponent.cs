using Microsoft.AspNetCore.Mvc;

public class ModalEstadoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int idTarea, int idUsuario, string nombre, string descripcion)
    {
        var mensaje = new ModalEstadoViewModel
        {
            IdTarea = idTarea,
            IdUsuario = idUsuario,
            Nombre = nombre,
            Descripcion = descripcion
        };
        return View(mensaje);
    }
}

