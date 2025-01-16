using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session;

using IUsuarioRepo;
public class SesionController : Controller
{
    private readonly ILogger<SesionController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    public SesionController(ILogger<SesionController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View(new LoguinViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoguinViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }
        try
        {
            Usuario usuario = _usuarioRepository.GetUsuario(model.Username, model.Password);

            if (usuario == null)
            {
                // Si el usuario no existe o las credenciales son incorrectas, envía un mensaje de error.
                ViewData["ErrorMessage"] = "El usuario no existe o los datos son incorrectos.";
                return View("Index", model); // Regresa a la vista Index con el mensaje de error.
            }
            HttpContext.Session.SetString("nombreUsuario", usuario.Nombre_de_usuario);
            HttpContext.Session.SetInt32("idUsuario", usuario.Id_usuario);
            HttpContext.Session.SetInt32("rolUsuario", (int)usuario.Rol_usuario);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            // Maneja excepciones y muestra un mensaje de error genérico.
            _logger.LogError($"Error en Login: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al iniciar sesión. Intente nuevamente.";
            return View("Index", model); // Regresa a la vista Index con el mensaje de error.
        }
    }
    [HttpGet]
    public IActionResult Logout()
    {
        try
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Logout: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al cerrar sesión.";
            return View("Error");
        }
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    // [HttpPost]
    // public IActionResult Register(string nombreUsuario, string password, int rol)
    // {
    //     try
    //     {
    //         Usuario usuario = new Usuario(0, nombreUsuario, password, (MisEnums.RolUsuario)rol);
    //         _usuarioRepository.Add(usuario);
    //         return RedirectToAction("Index");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError($"Error en Register: {ex.Message}");
    //         ViewData["ErrorMessage"] = "Hubo un problema al registrar el usuario.";
    //         return View("Error");
    //     }
    // }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}