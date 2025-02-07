using Microsoft.AspNetCore.Mvc;

public class TableroListaViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(List<TableroViewModel> tableros, string titulo, string claseCss)
    {
        int idPropietario = (int)HttpContext.Session.GetInt32("idUsuario");
        var model = new TableroListaViewModel
        {
            Tableros = tableros,
            Titulo = titulo,
            ClaseCss = claseCss,
            IdPropietario = idPropietario
        };
        return View(model);
    }
}