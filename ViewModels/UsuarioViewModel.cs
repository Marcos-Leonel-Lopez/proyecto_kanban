using System.ComponentModel.DataAnnotations;
public class UsuarioViewModel
{
    private int id_usuario;
    private string nombre;
    private string rol;
    public UsuarioViewModel(){}
    public UsuarioViewModel(int id, string nombre, string rol){
        this.id_usuario = id;
        this.nombre = nombre;
        this.rol = rol;
    }
    public UsuarioViewModel(Usuario usuario){
        this.id_usuario = usuario.Id_usuario;
        this.nombre = usuario.Nombre_de_usuario;
        this.rol = usuario.Rol_usuario.ToString();
    }
    public int Id_usuario { get => id_usuario; set => id_usuario = value; }
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre { get => nombre; set => nombre = value; }
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public string Rol { get => rol; set => rol = value; }
}