namespace ITableroRepo
{
    public interface ITableroRepository{
        Tablero Create(Tablero tablero);
        Tablero Update(Tablero tablero, int id_tablero);
        Tablero GetById(int id_tablero);
        List<Tablero> GetAll();
        List<Tablero> GetByUsuario(int id_usuario);
        List<Tablero> GetByParticipante(int id_usuario);
        bool Remove(int id_tablero);
        bool Participa(int id_usuario, int id_tablero);
    }
}