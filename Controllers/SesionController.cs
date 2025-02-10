using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session;

using Microsoft.Extensions.Logging;

using IUsuarioRepo;
public class SesionController : Controller
{
    private readonly ILogger<SesionController> _logger;
    private readonly ExceptionHandlerService _exceptionHandler;
    private readonly IUsuarioRepository _usuarioRepository;
    public SesionController(ILogger<SesionController> logger, ExceptionHandlerService exceptionHandler, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _exceptionHandler = exceptionHandler;
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
            HttpContext.Session.SetString("nombreUsuario", usuario.Nombre_de_usuario);
            HttpContext.Session.SetInt32("idUsuario", usuario.Id_usuario);
            HttpContext.Session.SetString("rolUsuario", usuario.Rol_usuario.ToString());
            HttpContext.Session.SetString("IsAuthenticated", "true");
            HttpContext.Session.SetString("AccessLevel", usuario.Rol_usuario.ToString());

            _logger.LogInformation($"El usuario {usuario.Nombre_de_usuario} ingresó correctamente");

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Login: {ex.Message.ToString()}");
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
            _logger.LogInformation("El usuario cerró sesión correctamente.");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Sesion", nameof(Logout));
        }
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}