using ITareaRepo;
using Microsoft.Data.Sqlite;

namespace TareaRepo
{
    public class TareaRepository : ITareaRepository
    {
        private readonly string _ConnectionString;
        public TareaRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        public Tarea Create(Tarea newTarea, int id_tablero)
        {
            Tarea tarea = newTarea;
            string query = "INSERT INTO Tarea(id_tablero,nombre,id_estado,descripcion,id_color,id_usuario_asignado) VALUES (@id_tablero,@nombre,@id_estado,@descripcion,@id_color,@id_usuario_asignado)";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    command.Parameters.AddWithValue("@nombre", tarea.Nombre);
                    command.Parameters.AddWithValue("@id_estado", Convert.ToInt32(tarea.Id_estado)); // Convertir enum a int
                    command.Parameters.AddWithValue("@descripcion", tarea.Descripcion ?? "Sin descripción"); // Valor por defecto si es null
                    command.Parameters.AddWithValue("@id_color", tarea.Id_color);

                    if (tarea.Id_usuario_asignado.HasValue)
                    {
                        command.Parameters.AddWithValue("@id_usuario_asignado", tarea.Id_usuario_asignado.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@id_usuario_asignado", DBNull.Value);
                    }
                    command.ExecuteNonQuery();
                }
                using (var command = new SqliteCommand("SELECT last_insert_rowid();", connection))
                {
                    tarea.Id_tarea = Convert.ToInt32(command.ExecuteScalar());
                }
                connection.Close();
            }
            return tarea;
        }
        // ser mas especifico con el nombre
        public Tarea UpdateEstado(Tarea tarea, int id_estado)
        {
            string query = @"UPDATE Tarea SET id_estado=@id_estado WHERE id_tarea=@id_tarea";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_estado", id_estado); // Convertir enum a int
                    command.Parameters.AddWithValue("@id_tarea", tarea.Id_tarea);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return tarea;
        }
        public Tarea UpdateUsuario(Tarea tarea, int? id_usuario)
        {
            string query = @"UPDATE Tarea SET id_usuario_asignado=@id_usuario WHERE id_tarea=@id_tarea";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    if (id_usuario.HasValue)
                    {
                        command.Parameters.AddWithValue("@id_usuario", id_usuario);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@id_usuario", DBNull.Value);
                    }
                    command.Parameters.AddWithValue("@id_tarea", tarea.Id_tarea);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return tarea;
        }
        public List<Tarea> GetAll()
        {
            List<Tarea> tareas = new List<Tarea>();
            string query = "SELECT * FROM Tarea";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tarea tarea = new Tarea
                            {
                                Id_tarea = reader.GetInt32(0),
                                Id_tablero = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Id_estado = (MisEnums.EstadoTarea)reader.GetInt32(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Id_color = reader.GetInt32(5),
                                Id_usuario_asignado = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            };
                            tareas.Add(tarea);
                        }
                    }
                }
                connection.Close();
            }
            return tareas;
        }
        public Tarea GetById(int id_tarea)
        {
            Tarea tarea = null;
            string query = "SELECT * FROM Tarea WHERE id_tarea=@id_tarea";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tarea", id_tarea);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tarea = new Tarea
                            {
                                Id_tarea = reader.GetInt32(0),
                                Id_tablero = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Id_estado = (MisEnums.EstadoTarea)reader.GetInt32(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Id_color = reader.GetInt32(5),
                                Id_usuario_asignado = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            };
                        }
                    }
                }
                connection.Close();
            }
            if (tarea == null)
            {
                throw new NoEncontradoException("Tarea", id_tarea);
            }
            return tarea;
        }
        public List<Tarea> GetByUsuario(int id_usuario)
        {
            List<Tarea> tareas = new List<Tarea>();
            string query = "SELECT * FROM Tarea WHERE id_usuario_asignado=@id_usuario";
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
                            Tarea tarea = new Tarea
                            {
                                Id_tarea = reader.GetInt32(0),
                                Id_tablero = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Id_estado = (MisEnums.EstadoTarea)reader.GetInt32(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Id_color = reader.GetInt32(5),
                                Id_usuario_asignado = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            };
                            tareas.Add(tarea);
                        }
                    }
                }
                connection.Close();
            }
            return tareas;
        }
        public List<Tarea> GetByTablero(int id_tablero)
        {
            List<Tarea> tareas = new List<Tarea>();
            string query = "SELECT * FROM Tarea WHERE id_tablero=@id_tablero";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tablero", id_tablero);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tarea tarea = new Tarea
                            {
                                Id_tarea = reader.GetInt32(0),
                                Id_tablero = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Id_estado = (MisEnums.EstadoTarea)reader.GetInt32(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Id_color = reader.GetInt32(5),
                                Id_usuario_asignado = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            };
                            tareas.Add(tarea);
                        }
                    }
                    connection.Close();
                }
            }
            return tareas;
        }
        public bool Remove(int id_tarea)
        {
            bool success = false;
            string query = "DELETE FROM Tarea WHERE id_tarea=@id_tarea";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_tarea", id_tarea);
                    success = command.ExecuteNonQuery() > 0;
                }
                connection.Close();
            }
            return success;
        }

        public void Unlink(int id_usuario)
        {
            string query = "UPDATE Tarea SET id_usuario_asignado = NULL WHERE id_usuario_asignado = @id_usuario;";

            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_usuario", id_usuario);
                    command.ExecuteNonQuery(); // No necesitamos el resultado
                }
            }
        }
    }
}