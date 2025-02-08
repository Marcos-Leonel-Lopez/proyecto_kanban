using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using IUsuarioRepo;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc.Filters;

namespace trabajo_final.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Index() // Igual al nombre de los archivos en carpeta "Views"
    {
        ViewData["rolUsuario"] = HttpContext.Session.GetString("rolUsuario");
        return View();
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult GetAll()
    {
        try
        {
            var usuarios = _usuarioRepository.GetAll();
            var viewModel = usuarios.Select(usuario => new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            }).ToList();
            return View(viewModel);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en GetAll: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetAll: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al obtener los usuarios.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult GetById(int idUsuario)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString() && HttpContext.Session.GetInt32("idUsuario").Value != idUsuario)
        {
            int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
            string nombre = HttpContext.Session.GetString("nombreUsuario");
            _logger.LogWarning($"Acceso denegado - Usuario: {nombre} - id {idUsuarioLogueado} intentó acceder a {HttpContext.Request.Path} sin permisos de administrador.");
            return View("Forbidden").WithStatusCode(403);
        }
        try
        {
            var usuario = _usuarioRepository.GetById(idUsuario);
            var viewModel = new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            return View(viewModel);
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.Message.ToString());
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = ex.Message,
                StatusCode = 404,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(404);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en GetById: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetById: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al obtener el usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new Usuario());
    }

    [HttpPost]
    [AccessAuthorize("Administrador")]
    public IActionResult Create(Usuario usuario)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }
        try
        {
            _usuarioRepository.Create(usuario);
            return RedirectToAction("GetAll");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en Create: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Create: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al crear el usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult ListToEdit()
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var usuarios = _usuarioRepository.GetAll();
            var viewModel = usuarios.Select(usuario => new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            }).ToList();
            return View(viewModel);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en ListToEdit: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en ListToEdit: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al obtener la lista de usuarios para editar.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult Delete(int idUsuario)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var usuario = _usuarioRepository.GetById(idUsuario);
            if (usuario == null)
            {
                ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
                return View("Error");
            }
            var viewModel = new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            ViewData["conTareas"] = _usuarioRepository.UserBusy(idUsuario);
            return View(viewModel);
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.Message.ToString());
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = ex.Message,
                StatusCode = 404,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(404);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en Delete: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Delete: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al obtener el usuario para eliminar.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador")]
    public IActionResult DeleteConfirmed(int idUsuario)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var usuario = _usuarioRepository.GetById(idUsuario);
            if (usuario == null)
            {
                ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
                return View("Error");
            }
            _usuarioRepository.Remove(idUsuario);
            return RedirectToAction("ListToEdit");
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.Message.ToString());
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = ex.Message,
                StatusCode = 404,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(404);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en DeleteConfirmed: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en DeleteConfirmed: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al eliminar el usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    // Verificar que el usuario sea admin o que el id de las ession coincida con el 'idUsuario'
    public IActionResult EditarPerfil(int idUsuario)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var usuario = _usuarioRepository.GetById(idUsuario);
            if (usuario == null)
            {
                ViewData["ErrorMessage"] = "Hubo un problema al intentar actualizar el usuario.";
                return View("Error");
            }
            var viewModel = new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            return View(viewModel);

        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.Message.ToString());
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = ex.Message,
                StatusCode = 404,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(404);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en EditarPerfil: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en EditarPerfil: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al cargar el perfil del usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador")] //admin
    public IActionResult EditarPerfil(UsuarioViewModel usuarioModif)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        if (!ModelState.IsValid)
        {
            return View(usuarioModif);
        }

        try
        {
            var usuarioExistente = _usuarioRepository.GetById(usuarioModif.Id_usuario);
            if (usuarioExistente == null)
            {
                ViewData["ErrorMessage"] = "Hubo un problema al intentar actualizar el usuario.";
                return View("Error");
            }
            var modif = _usuarioRepository.EditarPerfil(usuarioModif, usuarioModif.Id_usuario);
            if (!modif)
            {
                ViewData["ErrorMessage"] = "Hubo un problema al intentar actualizar el usuario.";
                return View("Error");
            }
            return RedirectToAction("ListToEdit");
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.Message.ToString());
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = ex.Message,
                StatusCode = 404,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(404);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en EditPerfil: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en EditPerfil: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al actualizar el perfil del usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult UpdatePass()
    {
        try
        {
            int idUsuario = HttpContext.Session.GetInt32("idUsuario").Value;
            ViewData["idUsuarioLogueado"] = idUsuario;
            return View(new UsuarioPasswordViewModel());
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en UpdatePass: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en UpdatePass: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al cargar el perfil del usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult UpdatePass(UsuarioPasswordViewModel passModif)
    {
        if (!ModelState.IsValid)
        {
            return View(passModif);
        }
        try
        {
            int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
            if (idUsuarioLogueado != passModif.IdUsuario)
            {
                string nombre = HttpContext.Session.GetString("nombreUsuario");
                _logger.LogWarning($"Acceso denegado - Usuario: {nombre} - id {idUsuarioLogueado} intentó acceder a {HttpContext.Request.Path} sin permisos de administrador.");
                return View("Forbidden").WithStatusCode(403);
            }
            var usuarioExistente = _usuarioRepository.GetById(idUsuarioLogueado);
            if (usuarioExistente == null)
            {
                ViewData["ErrorMessage"] = "Hubo un problema al intentar actualizar el usuario.";
                return View(passModif);  // No redirigir, solo retornar la vista
            }
            if (usuarioExistente.Password != passModif.Password)
            {
                ViewData["ErrorMessage"] = "La contraseña actual no coincide.";
                return View(passModif);  // No redirigir, solo retornar la vista
            }
            if (passModif.NewPassword != passModif.PasswordConfirm)
            {
                ViewData["ErrorMessage"] = "Las contraseñas no coinciden.";
                return View(passModif);  // No redirigir, solo retornar la vista
            }
            Usuario usuarioModif = new Usuario
            {
                Id_usuario = idUsuarioLogueado,
                Nombre_de_usuario = usuarioExistente.Nombre_de_usuario,
                Password = passModif.NewPassword,
                Rol_usuario = usuarioExistente.Rol_usuario
            };
            _usuarioRepository.UpdatePass(usuarioModif, idUsuarioLogueado);
            return RedirectToAction("Index", "Home");
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.Message.ToString());
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = ex.Message,
                StatusCode = 404,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(404);
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en UpdatePass: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema con la base de datos. Intente más tarde.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en UpdatePass: {ex.Message.ToString()}");
            var errorViewModel = new MyErrorViewModel
            {
                ErrorMessage = "Hubo un problema al actualizar el perfil del usuario.",
                StatusCode = 500,
                Controller = "Usuario"
            };
            return View("Error", errorViewModel).WithStatusCode(500);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
