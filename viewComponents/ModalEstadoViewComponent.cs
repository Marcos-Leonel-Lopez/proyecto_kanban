using Microsoft.AspNetCore.Mvc;

public class ModalEstadoViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int idTarea, int idUsuario, string nombre, string descripcion)
    {
        var mensaje = new ModalEstadoViewModel
        {
            Id_Tarea = idTarea,
            Id_Usuario = idUsuario,
            Nombre = nombre,
            Descripcion = descripcion
        };
        return View(mensaje);
    }
}

public class ModalEstadoViewModel
{
    public int Id_Tarea { get; set; }
    public int Id_Usuario { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
}