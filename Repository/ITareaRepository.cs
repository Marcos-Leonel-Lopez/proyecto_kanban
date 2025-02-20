namespace ITareaRepo
{
    public interface ITareaRepository{
        Tarea Create(Tarea tarea, int id_tablero);
        Tarea UpdateEstado(Tarea tarea, int id_estado);
        Tarea UpdateUsuario(Tarea tarea, int? id_usuario);
        List<Tarea> GetAll();
        Tarea GetById(int id_tarea);
        List<Tarea> GetByUsuario(int id_usuario);
        List<Tarea> GetByTablero(int id_tablero);
        bool Remove(int id_tarea);
        void Unlink(int id_usuario);

    }
}
