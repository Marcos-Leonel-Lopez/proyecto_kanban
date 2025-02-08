using System.ComponentModel.DataAnnotations;

public class UsuarioPasswordViewModel
{
    private int idUsuario;
    private string password;
    private string newPassword;
    private string passwordConfirm;
    public UsuarioPasswordViewModel() { }
    public UsuarioPasswordViewModel(int idUsuario, string password, string newPassword, string passwordConfirm)
    {
        this.idUsuario = idUsuario;
        this.password = password;
        this.newPassword = newPassword;
        this.passwordConfirm = passwordConfirm;
    }
    public int IdUsuario { get => idUsuario; set => idUsuario = value; }
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get => password; set => password = value; }
    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    public string NewPassword { get => newPassword; set => newPassword = value; }
    [Required(ErrorMessage = "Repetir la nueva contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    public string PasswordConfirm { get => passwordConfirm; set => passwordConfirm = value; }
}