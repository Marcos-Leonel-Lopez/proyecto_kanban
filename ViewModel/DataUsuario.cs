using System.ComponentModel.DataAnnotations;
public class DataUsuario
{
    private int id;
    private string nombre;
    private string rol;
    public DataUsuario(){}
    public DataUsuario(Usuario usuario){
        this.id = usuario.Id_usuario;
        this.nombre = usuario.Nombre_de_usuario;
        this.rol = usuario.Rol_usuario.ToString();
    }
    [Required]
    public int Id { get => id; set => id = value; }
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public string Rol { get => rol; set => rol = value; }
}