using ITableroRepo;
using Microsoft.Data.Sqlite;

namespace TableroRepo
{
    public class TableroRepository : ITableroRepository
    {
        private readonly string _ConnectionString;
        public TableroRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        public Tablero Create(Tablero newTablero){
            Tablero tablero  = newTablero;
            string query = "INSERT INTO Tablero (id_usuario_propietario,nombre,descripcion) VALUES (@id_propietario,@nombre,@descripcion)";
            using(var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using(var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_propietario", tablero.Id_usuario_propietario);
                    command.Parameters.AddWithValue("@nombre", tablero.Nombre);
                    command.Parameters.AddWithValue("@descripcion", tablero.Descripcion ?? "Sin descripción");
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand("SELECT last_insert_rowid();", connection))
                {
                    tablero.Id_tablero = Convert.ToInt32(command.ExecuteScalar());
                }
                connection.Close();
            }
            return tablero;
        }
        public Tablero Update(Tablero newTablero, int id_tablero){
            Tablero tablero = newTablero;
            string query = "UPDATE Tablero SET id_usuario_propietario=@id_propietario, nombre=@nombre, descripcion=@descripcion WHERE id_tablero=@id_tablero";
            using(var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using(var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_propietario", tablero.Id_usuario_propietario);
                    command.Parameters.AddWithValue("@nombre", tablero.Nombre);
                    command.Parameters.AddWithValue("@descripcion", tablero.Descripcion);
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return tablero;
        }
        public Tablero GetById(int id_tablero){
            Tablero tablero = null;
            string query = "SELECT * FROM Tablero WHERE id_tablero=@id_tablero";
            using(var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using(var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    using(var reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            tablero = new Tablero{
                                Id_tablero = reader.GetInt32(0),
                                Id_usuario_propietario = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Descripcion = reader.GetString(3)
                            };
                        }
                    }
                }
                connection.Close();
            }
            return tablero;
        }
        public List<Tablero> GetAll(){
            List<Tablero> tableros = new List<Tablero>();
            string query = "SELECT * FROM Tablero";
            using(var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using(var command = new SqliteCommand(query, connection))
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        tableros.Add(new Tablero{
                            Id_tablero = reader.GetInt32(0),
                            Id_usuario_propietario = reader.GetInt32(1),
                            Nombre = reader.GetString(2),
                            Descripcion = reader.GetString(3)
                        });
                    }
                }
                connection.Close();
            }
            return tableros;
        }
        public List<Tablero> GetForUsuario(int id_usuario){
            List<Tablero> tableros = new List<Tablero>();
            string query = "SELECT * FROM Tablero WHERE id_usuario_propietario=@id_usuario";
            using(var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using(var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_usuario", id_usuario);
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            tableros.Add(new Tablero{
                                Id_tablero = reader.GetInt32(0),
                                Id_usuario_propietario = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Descripcion = reader.GetString(3)
                            });
                        }
                    }
                }
                connection.Close();
            }
            return tableros;
        }
        public bool Remove(int id_tablero){
            bool success = false;
            string query = "DELETE FROM Tablero WHERE id_tablero=@id_tablero";
            using(var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using(var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    success = command.ExecuteNonQuery() > 0;
                }
                connection.Close();
            }
            return success;
        }
    }
}