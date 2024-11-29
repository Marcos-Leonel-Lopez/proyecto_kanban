namespace ITableroRepo
{
    public interface ITableroRepository{
        Tablero Create(Tablero tablero);
        Tablero Update(Tablero tablero, int id_tablero);
        Tablero GetById(int id_tablero);
        List<Tablero> GetAll();
        List<Tablero> GetForUsuario(int id_usuario);
        bool Remove(int id_tablero);
    }
}