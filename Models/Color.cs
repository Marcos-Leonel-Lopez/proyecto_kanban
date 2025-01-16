public class Color
{
    private int id_color;
    private string nombre;
    private string hex;
    public Color(){}
    public Color(int id_color, string nombre, string hex)
    {
        this.id_color = id_color;
        this.nombre = nombre;
        this.hex = hex;
    }

    public int Id_color { get => id_color; set => id_color = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Hex { get => hex; set => hex = value; }
}