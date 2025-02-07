using Microsoft.AspNetCore.Mvc;

public class ModalConfirmacionViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int id_tablero, string nombre_tablero)
    {
        var mensaje = new ModalConfirmacionViewModel
        {
            IdTablero = id_tablero,
            NombreTablero = nombre_tablero
        };
        return View(mensaje);
    }
    
}

public class ModalConfirmacionViewModel
{
    public int IdTablero { get; set; }
    public string NombreTablero { get; set; }
}