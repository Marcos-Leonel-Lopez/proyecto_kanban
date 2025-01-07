using System.ComponentModel.DataAnnotations;
public class TableroViewModel
{
    private int id;
    private string nombre;
    private string descripcion;
    private string nombrePropietario;
    private int id_propietario;
    public TableroViewModel(){}
    public TableroViewModel(int id, string nombre, string descripcion, string nombrePropietario, int id_propietario){
        this.id = id;
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.nombrePropietario = nombrePropietario;
        this.id_propietario = id_propietario;
    }
    [Required]
    public int Id { get => id; set => id = value; }
    [Required(ErrorMessage = "El nombre de tablero es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de tablero no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string NombrePropietario { get => nombrePropietario; set => nombrePropietario = value; }
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public int Id_propietario { get => id_propietario; set => id_propietario = value; }
}