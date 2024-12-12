using System.ComponentModel.DataAnnotations;
public class Usuario
{
    private int id_usuario;

    private string nombre_de_usuario;

    private string password;

    private MisEnums.RolUsuario rol_usuario;
    public Usuario() { }
    public Usuario(int id_usuario, string nombre_de_usuario, string password, MisEnums.RolUsuario rol)
    {
        this.id_usuario = id_usuario;
        this.nombre_de_usuario = nombre_de_usuario;
        this.password = password;
        this.rol_usuario = rol;
    }

    public int Id_usuario { get => id_usuario; set => id_usuario = value; }
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre_de_usuario { get => nombre_de_usuario; set => nombre_de_usuario = value; }
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    public string Password { get => password; set => password = value; }
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public MisEnums.RolUsuario Rol_usuario { get => rol_usuario; set => rol_usuario = value; }
}