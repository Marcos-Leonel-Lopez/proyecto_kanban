using IColorRepo;
using Microsoft.Data.Sqlite;

namespace ColorRepo
{
    public class ColorRepository : IColorRepository
    {
        private readonly string _ConnectionString;
        public ColorRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        public List<Color> GetAll()
        {
            try
            {
                List<Color> colores = new List<Color>();
                string query = "SELECT * FROM Color";
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                colores.Add(new Color
                                {
                                    Id_color = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Hex = reader.GetString(2)
                                });
                            }
                        }
                    }
                    connection.Close();
                }
                return colores;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetAll: {ex.Message}");
                throw;
            }
        }
    }
}