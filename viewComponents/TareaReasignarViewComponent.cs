using Microsoft.AspNetCore.Mvc;

public class TareaReasignarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(TareaEnTableroViewModel tareaActual, bool esPropietario, List<UsuarioViewModel> usuarios){
        var habilitar = new TareaReasignarViewModel
        {
            Tarea = tareaActual,
            EsPropietario = esPropietario,
            Usuarios = usuarios
        };
    return View(habilitar);
    }
}