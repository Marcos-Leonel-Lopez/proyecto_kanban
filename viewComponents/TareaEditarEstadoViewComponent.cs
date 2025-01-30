using Microsoft.AspNetCore.Mvc;

public class TareaEditarEstadoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(TareaEnTableroViewModel tareaActual, int idUsuarioLogueadoActual){
        var habilitar = new TareaEditarEstadoViewModel
        {
            Tarea = tareaActual,
            IdUsuarioLogueado = idUsuarioLogueadoActual
        };
    return View(habilitar);
    }
}