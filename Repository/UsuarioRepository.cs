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
        public Usuario Update(Usuario newUsuario, int id)
        {
            Usuario usuario = newUsuario;
            string query = "UPDATE Usuario SET nombre_de_usuario=@nombre, password=@password, rol_usuario=@rol WHERE id_usuario=@id";
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre_de_usuario);
                    command.Parameters.AddWithValue("@password", usuario.Password);
                    command.Parameters.AddWithValue("@rol", usuario.Rol_usuario);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return usuario;
        }
        public List<Usuario> GetAll(){
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
                                Rol_usuario = (MisEnums.RolUsuario)reader.GetInt32(3) //Parseo el int a enum
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
                connection.Close();
            }
            return usuarios;
        }
        public Usuario GetById(int id_usuario){
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
                                Rol_usuario = (MisEnums.RolUsuario)reader.GetInt32(3) //Parseo el int a enum
                            };
                        }
                    }
                }
                connection.Close();
            }
            return usuario;
        }
        public bool Remove(int id){
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
        public Usuario UpdatePass(Usuario usuario, int id){
            string query = "UPDATE Usuario SET passsword = @pass WHERE id_usuario = @id";
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
    }
}