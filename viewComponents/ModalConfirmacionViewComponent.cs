using Microsoft.AspNetCore.Mvc;

public class ModalConfirmacionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int idTablero, string nombreTablero)
    {
        var mensaje = new ModalConfirmacionViewModel
        {
            IdTablero = idTablero,
            NombreTablero = nombreTablero
        };
        return View(mensaje);
    }
    
}

