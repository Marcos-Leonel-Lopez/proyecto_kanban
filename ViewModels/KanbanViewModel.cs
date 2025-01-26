public class KanbanViewModel
{
    public List<TareaEnTableroViewModel> Tareas { get; set; } = new List<TareaEnTableroViewModel>();
    public TareaEnTableroViewModel TareaModificada { get; set; } = new TareaEnTableroViewModel();
    public bool esPropietario{ get; set; } // determina si puede modificar la tarea
}
