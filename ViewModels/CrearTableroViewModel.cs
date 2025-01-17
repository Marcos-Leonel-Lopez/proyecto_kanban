using System.ComponentModel.DataAnnotations;
public class CrearTableroViewModel
{
    private int id_usuario_propietario;
    private string nombre;
    private string descripcion;
    public CrearTableroViewModel() { }
    public CrearTableroViewModel(int id_usuario_propietario, string nombre, string descripcion)
    {
        this.id_usuario_propietario = id_usuario_propietario;
        this.nombre = nombre;
        this.descripcion = descripcion;
    }
    [Required(ErrorMessage = "El nombre del tablero es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre del tablero no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    public int Id_usuario_propietario { get => id_usuario_propietario; set => id_usuario_propietario = value; }
}