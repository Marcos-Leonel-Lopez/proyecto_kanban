using Microsoft.AspNetCore.Mvc;

public class TableroListaViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(List<TableroViewModel> tableros, string titulo, string claseCss)
    {
        var model = new TableroListaViewModel
        {
            Tableros = tableros,
            Titulo = titulo,
            ClaseCss = claseCss
        };
        return View(model);
    }
}