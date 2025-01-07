using System.ComponentModel.DataAnnotations;
public class UsuarioViewModel
{
    private int id;
    private string nombre;
    private string rol;
    public UsuarioViewModel(){}
    public UsuarioViewModel(int id, string nombre, string rol){
        this.id = id;
        this.nombre = nombre;
        this.rol = rol;
    }
    [Required]
    public int Id { get => id; set => id = value; }
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public string Rol { get => rol; set => rol = value; }
}