#nullable enable
using System.ComponentModel.DataAnnotations;
public class Tarea
{
    private int id_tarea;
    private int id_tablero;
    private string nombre;
    private string? descripcion;
    private int id_color;
    private MisEnums.EstadoTarea id_estado;
    private int? id_usuario_asignado;
    public Tarea(){}
    public Tarea(int id_tarea, int id_tablero, string nombre, string? descripcion, int id_color, MisEnums.EstadoTarea id_estado, int? id_usuario_asignado)
    {
        this.id_tarea = id_tarea;
        this.id_tablero = id_tablero;
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.id_color = id_color;
        this.id_estado = id_estado;
        this.id_usuario_asignado = id_usuario_asignado;
    }

    public int Id_tarea { get => id_tarea; set => id_tarea = value; }
    public int Id_tablero { get => id_tablero; set => id_tablero = value; }
    [Required(ErrorMessage = "El nombre de la tarea es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    public string? Descripcion { get => descripcion; set => descripcion = value; }
    public int Id_color { get => id_color; set => id_color = value; }
    public MisEnums.EstadoTarea Id_estado { get => id_estado; set => id_estado = value; }
    public int? Id_usuario_asignado { get => id_usuario_asignado; set => id_usuario_asignado = value; }
}