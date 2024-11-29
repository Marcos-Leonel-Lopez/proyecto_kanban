public class Usuario
{
    private int id_usuario;
    private string nombre_de_usuario;
    private string password;
    private MisEnums.RolUsuario rol_usuario;
    public Usuario(){}
    public Usuario(int id_usuario, string nombre_de_usuario, string password, MisEnums.RolUsuario rol){
        this.id_usuario = id_usuario;
        this.nombre_de_usuario = nombre_de_usuario;
        this.password = password;
        this.rol_usuario = rol;
    }

    public int Id_usuario { get => id_usuario; set => id_usuario = value; }
    public string Nombre_de_usuario { get => nombre_de_usuario; set => nombre_de_usuario = value; }
    public string Password { get => password; set => password = value; }
    public MisEnums.RolUsuario Rol_usuario { get => rol_usuario; set => rol_usuario = value; }
}