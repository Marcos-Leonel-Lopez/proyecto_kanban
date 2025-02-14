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
            Usuario usuario = newUsuario;
            string query = "INSERT INTO Usuario (nombre_de_usuario,password,rol_usuario) VALUES (@nombre,@password,@rol)";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre_de_usuario);
                    command.Parameters.AddWithValue("@password", BCryptService.HashPassword(newUsuario.Password));
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
        public bool EditarPerfil(UsuarioViewModel newUsuario, int id)
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
        public List<Usuario> GetAll()
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
        public Usuario GetById(int id_usuario)
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
            if (usuario == null)
            {
                throw new NoEncontradoException("Usuario", id_usuario);
            }
            return usuario;
        }
        public bool Remove(int id)
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
        public Usuario UpdatePass(Usuario usuario, int id)
        {
            string hash = BCryptService.HashPassword(usuario.Password);
            string query = "UPDATE Usuario SET password = @pass WHERE id_usuario = @id";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@pass", hash);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return usuario;
        }
        public Usuario GetUsuario(string username, string password)
        {
            Usuario usuario = null;
            string query = "SELECT * FROM Usuario WHERE nombre_de_usuario = @nombre";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id_usuario = reader.GetInt32(0),
                                Nombre_de_usuario = reader.GetString(1),
                                Password = reader.GetString(2), // La contraseña encriptada de la base de datos
                                Rol_usuario = (MisEnums.RolUsuario)reader.GetInt32(3)
                            };
                        }
                    }
                }
                connection.Close();
            }
            if (usuario == null || !BCryptService.VerificarPassword(password, usuario.Password))  // Comparación segura
            {
                throw new AccesoDenegadoException(username, password);
            }

            return usuario;
        }
        public bool UserBusy(int id_usuario)
        {
            bool busy = false;
            string query = @"
            SELECT 
                U.id_usuario,
                (SELECT COUNT(*) FROM Tarea Tr WHERE Tr.id_usuario_asignado = U.id_usuario) AS TotalTareas,
                (SELECT COUNT(*) FROM Tablero Tb WHERE Tb.id_usuario_propietario = U.id_usuario) AS TotalTableros,
                (
                    (SELECT COUNT(*) FROM Tarea Tr WHERE Tr.id_usuario_asignado = U.id_usuario) +
                    (SELECT COUNT(*) FROM Tablero Tb WHERE Tb.id_usuario_propietario = U.id_usuario)
                ) AS Total
            FROM usuario U
            WHERE U.id_usuario = @id;";

            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id_usuario);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Asegúrate de que el índice 3 corresponde a la columna 'Total'
                            busy = reader.GetInt32(reader.GetOrdinal("Total")) > 0;
                        }
                    }
                }
            }
            return busy;
        }

        // public bool scriptHash()
        // {
        //     try
        //     {
        //         string query = "SELECT password FROM Usuario";
        //         using (var connection = new SqliteConnection(_ConnectionString))
        //         {
        //             connection.Open();
        //             using (var command = new SqliteCommand(query, connection))
        //             {
        //                 using (var reader = command.ExecuteReader())
        //                 {
        //                     while (reader.Read())
        //                     {
        //                         string hash = BCryptService.HashPassword(reader.GetString(0));
        //                         string updateQuery = "UPDATE Usuario SET password = @pass WHERE password = @oldPass";
        //                         using (var updateCommand = new SqliteCommand(updateQuery, connection))
        //                         {
        //                             updateCommand.Parameters.AddWithValue("@pass", hash);
        //                             updateCommand.Parameters.AddWithValue("@oldPass", reader.GetString(0));
        //                             updateCommand.ExecuteNonQuery();
        //                         }
        //                     }
        //                 }
        //             }
        //             connection.Close();
        //         }
        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.Error.WriteLine($"Error en scriptHash: {ex.ToString()}");
        //         return false;
        //     }
        // }
    }
}
