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
            try
            {
                Tarea tarea = newTarea;
                string query = "INSERT INTO Tarea(id_tablero,nombre,id_estado,descripcion,id_color,id_usuario_asignado) VALUES (@id_tablero,@nombre,@id_estado,@descripc,@id_color,@id_usuario_asign)";
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
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en Create: {ex.Message}");
                throw;
            }

        }
        public Tarea Update(Tarea tarea, int id_tarea)
        {
            try
            {
                string query = "UPDATE Tarea SET id_tablero=@id_tablero, nombre=@nombre, id_estado=@id_estado, " +
               "descripcion=@descripcion, id_color=@id_color, id_usuario_asignado=@id_usuario_asignado " +
               "WHERE id_tarea=@id_tarea";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_tablero", tarea.Id_tablero);
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

                        command.Parameters.AddWithValue("@id_tarea", id_tarea);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return tarea;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en Update: {ex.Message}");
                throw;
            }

        }
        public Tarea GetById(int id_tarea)
        {
            try
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
                return tarea;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetById: {ex.Message}");
                throw;
            }
        }
        public List<Tarea> GetByUsuario(int id_usuario)
        {
            try
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
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetByUsuario: {ex.Message}");
                throw;
            }

        }
        public List<Tarea> GetByTablero(int id_tablero)
        {
            try
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
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetByTablero: {ex.Message}");
                throw;
            }
        }
        public bool Remove(int id_tarea)
        {
            try
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
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en Remove: {ex.Message}");
                throw;
            }
        }
        public bool AssignUserToTask(int id_usuario, int id_tarea)
        {
            try
            {
                bool success = false;
                string query = "UPDATE Tarea SET id_usuario_asignado=@id_usuario WHERE id_tarea=@id_tarea";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_usuario", id_usuario);
                        command.Parameters.AddWithValue("@id_tarea", id_tarea);
                        success = command.ExecuteNonQuery() > 0;
                    }
                    connection.Close();
                }
                return success;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en AssignUserToTask: {ex.Message}");
                throw;
            }
        }
    }
}