
using System.ComponentModel.DataAnnotations;
public class NuevaTareaViewModel
{
    private string nombre;
    private string descripcion;
    private int id_tablero;
    private int? id_color;
    private int? id_usuario_asignado;
    public NuevaTareaViewModel() { }
    public NuevaTareaViewModel(string nombre, string descripcion, int id_tablero, int id_color,  int id_usuario_asignado)
    {
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.id_tablero = id_tablero;
        this.id_color = id_color;
        this.id_usuario_asignado = id_usuario_asignado;
    }
    [Required(ErrorMessage = "El nombre de la tarea es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    [Required(ErrorMessage = "Asignar un tablero es obligatorio.")]
    public int Id_tablero { get => id_tablero; set => id_tablero = value; }
    public int? Id_color { get => id_color; set => id_color = value; }
    public int? Id_usuario_asignado { get => id_usuario_asignado; set => id_usuario_asignado = value; }
}
public class TareasCreadasViewModel{
    public string Nombre_tablero { get; set; }
    public List<TareaListaViewModel> Tareas { get; set; }
}

public class CrearTareaViewModel
{
    private NuevaTareaViewModel nuevaTarea;
    private List<TableroViewModel> tableros;
    private List<Color> colores;
    private List<UsuarioViewModel> usuarios;
    private List<TareasCreadasViewModel> tareasCreadas;
    public CrearTareaViewModel() { }

    public NuevaTareaViewModel NuevaTarea { get => nuevaTarea; set => nuevaTarea = value; }
    public List<TableroViewModel> Tableros { get => tableros; set => tableros = value; }
    public List<Color> Colores { get => colores; set => colores = value; }
    public List<UsuarioViewModel> Usuarios { get => usuarios; set => usuarios = value; }
    public List<TareasCreadasViewModel> TareasCreadas { get => tareasCreadas; set => tareasCreadas = value; }
}