public class ListarTablerosUsuarioViewModel
{
    public int Id_tablero { get; set; }
    public string Nombre { get; set; }
    public List<TareaListaViewModel> Tareas { get; set; }
}