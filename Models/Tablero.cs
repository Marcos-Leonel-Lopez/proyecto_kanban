#nullable enable
public class Tablero
{
    private int id_tablero;
    private int id_usuario_propietario;
    private string nombre;
    private string descripcion;
    public Tablero(){}
    public Tablero(int id_tablero, int id_usuario_propietario, string nombre, string descripcion)
    {
        this.id_tablero = id_tablero;
        this.id_usuario_propietario = id_usuario_propietario;
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    public int Id_tablero { get => id_tablero; set => id_tablero = value; }
    public int Id_usuario_propietario { get => id_usuario_propietario; set => id_usuario_propietario = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
}
