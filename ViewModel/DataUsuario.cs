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

    public int Id { get => id; set => id = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Rol { get => rol; set => rol = value; }
}