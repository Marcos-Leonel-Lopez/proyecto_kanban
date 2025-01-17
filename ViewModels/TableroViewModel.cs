using System.ComponentModel.DataAnnotations;
public class TableroViewModel
{
    private int id_tablero;
    private string nombre;
    private string descripcion;
    private int id_propietario;
    private string nombrePropietario;
    public TableroViewModel(){}
    public TableroViewModel(int id, string nombre, string descripcion, int id_propietario,string nombrePropietario){
        this.id_tablero = id;
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.id_propietario = id_propietario;
        this.nombrePropietario = nombrePropietario;
    }
    public TableroViewModel(Tablero tablero,string nombrePropietario){
        this.id_tablero = tablero.Id_tablero;
        this.nombre = tablero.Nombre;
        this.descripcion = tablero.Descripcion;
        this.id_propietario = tablero.Id_usuario_propietario;
        this.nombrePropietario = nombrePropietario;
    }
    public int Id_tablero { get => id_tablero; set => id_tablero = value; }
    [Required(ErrorMessage = "El nombre de tablero es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de tablero no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public int Id_propietario { get => id_propietario; set => id_propietario = value; }
    public string NombrePropietario { get => nombrePropietario; set => nombrePropietario = value; }
}