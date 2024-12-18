namespace ITareaRepo
{
    public interface ITareaRepository{
        Tarea Create(Tarea tarea, int id_tablero);
        Tarea Update(Tarea tarea, int id_tarea);
        Tarea GetById(int id_tarea);
        List<Tarea> GetForUsuario(int id_usuario);
        List<Tarea> GetForTablero(int id_tablero);
        bool Remove(int id_tarea);
        bool AssignUserToTask(int id_usuario, int id_tarea);
    }
}
