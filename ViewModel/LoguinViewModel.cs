using System.ComponentModel.DataAnnotations;

public class LoguinViewModel
{
    private string username;
    private string password;
    public LoguinViewModel() { }
    public LoguinViewModel(string username, int idUsuario, string password)
    {
        this.username = username;
        this.password = password;
    }
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string Username { get => username; set => username = value; }
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get => password; set => password = value; }
}