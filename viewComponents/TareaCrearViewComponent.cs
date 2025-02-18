using Microsoft.AspNetCore.Mvc;

public class TareaCrearViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CrearTareaViewModel model, int? idTablero = null)
    {
        if (idTablero.HasValue)
        {
            model.NuevaTarea.Id_tablero = idTablero.Value;
            model.Tableros = new List<TableroViewModel>(); // Se desactiva el selector
        }
        return View(model);
    }
}