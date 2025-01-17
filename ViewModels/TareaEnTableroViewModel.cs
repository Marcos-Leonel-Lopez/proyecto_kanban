public class TareaEnTableroViewModel
{
    private int id_tarea;
    private string nombre;
    private string descripcion;
    private MisEnums.EstadoTarea id_estado;
    private int? id_usuario_asignado;
    public TareaEnTableroViewModel() { }
    public TareaEnTableroViewModel(int id_tarea, string nombre, string descripcion, MisEnums.EstadoTarea id_estado, int? id_usuario_asignado)
    {
        this.id_tarea = id_tarea;
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.id_estado = id_estado;
        this.id_usuario_asignado = id_usuario_asignado;
    }

    public int Id_tarea { get => id_tarea; set => id_tarea = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    public MisEnums.EstadoTarea Id_estado { get => id_estado; set => id_estado = value; }
    public int? Id_usuario_asignado { get => id_usuario_asignado; set => id_usuario_asignado = value; }
}