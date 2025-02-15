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
        public Tablero Create(Tablero newTablero)
        {
            Tablero tablero = newTablero;
            string query = "INSERT INTO Tablero (id_usuario_propietario,nombre,descripcion) VALUES (@id_propietario,@nombre,@descripcion)";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
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
        public Tablero Update(Tablero newTablero, int id_tablero)
        {
            Tablero tablero = newTablero;
            string query = "UPDATE Tablero SET id_usuario_propietario=@id_propietario, nombre=@nombre, descripcion=@descripcion WHERE id_tablero=@id_tablero";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
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
        public Tablero GetById(int id_tablero)
        {
            Tablero tablero = null;
            string query = "SELECT * FROM Tablero WHERE id_tablero=@id_tablero";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tablero = new Tablero
                            {
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
            if (tablero == null)
            {
                throw new NoEncontradoException("Tablero", id_tablero);
            }
            return tablero;
        }
        public List<Tablero> GetAll()
        {
            List<Tablero> tableros = new List<Tablero>();
            string query = "SELECT * FROM Tablero";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableros.Add(new Tablero
                        {
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
        public List<Tablero> GetByUsuario(int id_usuario)
        {
            List<Tablero> tableros = new List<Tablero>();
            string query = "SELECT * FROM Tablero WHERE id_usuario_propietario=@id_usuario";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_usuario", id_usuario);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableros.Add(new Tablero
                            {
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
        public List<Tablero> GetByParticipante(int id_usuario)
        {
            List<Tablero> tableros = new List<Tablero>();
            string query = @"
                SELECT DISTINCT Tb.id_tablero, Tb.id_usuario_propietario, Tb.nombre, Tb.descripcion 
                FROM Tablero Tb
                JOIN Tarea Tr ON Tb.id_tablero = Tr.id_tablero
                WHERE Tr.id_usuario_asignado = @id_usuario AND Tb.id_usuario_propietario != @id_usuario;";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_usuario", id_usuario);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableros.Add(new Tablero
                            {
                                Id_tablero = reader.GetInt32(reader.GetOrdinal("id_tablero")),
                                Id_usuario_propietario = reader.GetInt32(reader.GetOrdinal("id_usuario_propietario")),
                                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                Descripcion = reader.GetString(reader.GetOrdinal("descripcion"))
                            });
                        }
                    }
                }
            }
            return tableros;
        }
        public bool Remove(int id_tablero)
        {
            bool success = false;
            string query = "DELETE FROM Tablero WHERE id_tablero=@id_tablero";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    success = command.ExecuteNonQuery() > 0;
                }
                connection.Close();
            }
            return success;
        }

        public bool Participa(int id_usuario, int id_tablero){
            int count = 0;
            string query = "SELECT COUNT(1) FROM Tarea WHERE id_tablero = @id_Tb AND id_usuario_asignado = @id_U;";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                 using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_Tb", id_tablero);
                    command.Parameters.AddWithValue("@id_U", id_usuario);
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
                connection.Close();
            }
            return count > 0;
        }
    }
}