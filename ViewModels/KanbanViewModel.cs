public class KanbanViewModel
{
    public List<TareaEnTableroViewModel> Tareas { get; set; } = new List<TareaEnTableroViewModel>();
    public TareaEnTableroViewModel TareaModificada { get; set; } = new TareaEnTableroViewModel();
    public bool EsPropietario{ get; set; } // determina si puede modificar la tarea
    public List<UsuarioViewModel> Usuarios { get; set; } = new List<UsuarioViewModel>();
    public int IdPropietario { get; set; } // para realizar verificaciones
    public CrearTareaViewModel NuevaTarea {get; set;} = new CrearTareaViewModel();
}
