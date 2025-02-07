using Microsoft.AspNetCore.Mvc;
public class ModalReasignarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int idTarea, List<UsuarioViewModel> usuarios)
    {
        var mensaje = new ModalReasignarViewModel
        {
            Id_Tarea = idTarea,
            Usuarios = usuarios
        };
        return View(mensaje);
    }
}

public class ModalReasignarViewModel
{
    public int Id_Tarea { get; set; }
    public List<UsuarioViewModel> Usuarios { get; set; }
}