using Microsoft.AspNetCore.Mvc;

public class TareaListadoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ListarTablerosUsuarioViewModel tareas)
    {
        return View(tareas);
    }
}
