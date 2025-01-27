namespace ITareaRepo
{
    public interface ITareaRepository{
        Tarea Create(Tarea tarea, int id_tablero);
        Tarea UpdateEstado(Tarea tarea, int id_estado);
        Tarea UpdateUsuario(Tarea tarea, int? id_usuario);
        Tarea GetById(int id_tarea);
        List<Tarea> GetByUsuario(int id_usuario);
        List<Tarea> GetByTablero(int id_tablero);
        bool Remove(int id_tarea);

    }
}
