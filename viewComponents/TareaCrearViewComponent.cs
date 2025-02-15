using Microsoft.AspNetCore.Mvc;

public class TareaCrearViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CrearTareaViewModel model, int? tableroId = null)
    {
        if (tableroId.HasValue)
        {
            model.NuevaTarea.Id_tablero = tableroId.Value;
            model.Tableros = new List<TableroViewModel>(); // Se desactiva el selector
        }
        return View(model);
    }
}