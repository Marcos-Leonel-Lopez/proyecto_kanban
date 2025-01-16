using IUsuarioRepo;
using Microsoft.Data.Sqlite;

namespace UsuarioRepo
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _ConnectionString;
        public UsuarioRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        public Usuario Create(Usuario newUsuario)
        {
            try
            {
                Usuario usuario = newUsuario;
                string query = "INSERT INTO Usuario (nombre_de_usuario,password,rol_usuario) VALUES (@nombre,@password,@rol)";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", usuario.Nombre_de_usuario);
                        command.Parameters.AddWithValue("@password", usuario.Password);
                        command.Parameters.AddWithValue("@rol", Convert.ToInt32(usuario.Rol_usuario));
                        command.ExecuteNonQuery();
                    }
                    using (var command = new SqliteCommand("SELECT last_insert_rowid();", connection))
                    {
                        usuario.Id_usuario = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                }
                return usuario;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en Create: {ex.Message}");
                throw;
            }
        }
        public bool EditarPerfil(UsuarioViewModel newUsuario, int id)
        {
            try
            {
                UsuarioViewModel usuario = newUsuario;
                string query = "UPDATE Usuario SET nombre_de_usuario=@nombre, rol_usuario=@rol WHERE id_usuario=@id";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@rol", usuario.Rol);
                        command.Parameters.AddWithValue("@id", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en EditarPerfil: {ex.Message}");
                return false;
            }
        }
        public List<Usuario> GetAll()
        {
            try
            {
                List<Usuario> usuarios = new List<Usuario>();
                string query = "SELECT * FROM Usuario";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuario usuario = new Usuario
                                {
                                    Id_usuario = reader.GetInt32(0),
                                    Nombre_de_usuario = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Rol_usuario = (MisEnums.RolUsuario)reader.GetInt32(3)
                                };
                                usuarios.Add(usuario);
                            }
                        }
                    }
                    connection.Close();
                }
                return usuarios;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetAll: {ex.Message}");
                throw;
            }
        }
        public Usuario GetById(int id_usuario)
        {
            try
            {
                Usuario usuario = null;
                string query = "SELECT * FROM Usuario WHERE id_usuario = @id_usuario";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_usuario", id_usuario);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario = new Usuario
                                {
                                    Id_usuario = reader.GetInt32(0),
                                    Nombre_de_usuario = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Rol_usuario = (MisEnums.RolUsuario)reader.GetInt32(3)
                                };
                            }
                        }
                    }
                    connection.Close();
                }
                return usuario;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetById: {ex.Message}");
                throw;
            }
        }
        public bool Remove(int id)
        {
            try
            {
                bool success = false;
                string query = "DELETE FROM Usuario WHERE id_usuario = @id";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        success = command.ExecuteNonQuery() > 0;
                    }
                    connection.Close();
                }
                return success;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en Remove: {ex.Message}");
                return false;
            }
        }
        public Usuario UpdatePass(Usuario usuario, int id)
        {
            try
            {
                string query = "UPDATE Usuario SET password = @pass WHERE id_usuario = @id";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@pass", usuario.Password);
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return usuario;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en UpdatePass: {ex.Message}");
                throw;
            }
        }
        public Usuario GetUsuario(string username, string password){
            try
            {
                Usuario usuario = null;
                string query = "SELECT * FROM Usuario WHERE nombre_de_usuario = @nombre AND password = @pass";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", username);
                        command.Parameters.AddWithValue("@pass", password);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario = new Usuario
                                {
                                    Id_usuario = reader.GetInt32(0),
                                    Nombre_de_usuario = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Rol_usuario = (MisEnums.RolUsuario)reader.GetInt32(3)
                                };
                            }
                        }
                    }
                    connection.Close();
                }
                return usuario;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetUsuario: {ex.Message}");
                throw;
            }
        }
    }
}
