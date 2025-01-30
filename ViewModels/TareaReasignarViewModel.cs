public class TareaReasignarViewModel
{
    public TareaEnTableroViewModel Tarea {get; set;}
    public bool EsPropietario {get; set;}
    public List<UsuarioViewModel> Usuarios { get; set; }
}