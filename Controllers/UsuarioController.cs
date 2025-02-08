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
    [VerificarSesion]
    public IActionResult Index() // Igual al nombre de los archivos en carpeta "Views"
    {
        ViewData["rolUsuario"] = HttpContext.Session.GetString("rolUsuario");
        return View();
    }
    [HttpGet]
    [VerificarSesion(requireAdmin: true)]
    public IActionResult GetAll()
    {
        // if(HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        // {
        //     int idUsuario = HttpContext.Session.GetInt32("idUsuario").Value;
        //     string nombre = HttpContext.Session.GetString("nombreUsuario");
        //     _logger.LogWarning($"Acceso denegado - Usuario: {nombre} - id {idUsuario} intentó acceder a {HttpContext.Request.Path} sin permisos de administrador.");
        //     return View("Forbidden").WithStatusCode(403);
        // }
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
            _logger.LogError($"Error en base de datos en GetAll CONTROLADOR: {ex.Message.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetAll: {ex.Message.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener los usuarios.";
            return View("Error");
        }
    }
    [HttpGet]
    [VerificarSesion]
    public IActionResult GetById(int idUsuario)
    {
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
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en GetById: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetById: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener el usuario.";
            return View("Error");
        }
    }
    [HttpGet]
    [VerificarSesion]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new Usuario());
    }

    [HttpPost]
    [VerificarSesion]
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
            _logger.LogError($"Error en base de datos en Create: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Create: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al crear el usuario.";
            return View("Error");
        }
    }
    [HttpGet]
    [VerificarSesion]
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
            _logger.LogError($"Error en base de datos en ListToEdit: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en ListToEdit: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener la lista de usuarios para editar.";
            return View("Error");
        }
    }
    [HttpGet]
    [VerificarSesion]
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
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en Delete: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Delete: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener el usuario para eliminar.";
            return View("Error");
        }
    }
    [HttpPost]
    [VerificarSesion]
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
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en DeleteConfirmed: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en DeleteConfirmed: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al eliminar el usuario.";
            return View("Error");
        }
    }
    [HttpGet]
    [VerificarSesion]
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
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en EditarPerfil: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en EditarPerfil: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al cargar el perfil del usuario.";
            return View("Error");
        }
    }
    [HttpPost]
    [VerificarSesion] //admin
    public IActionResult EditarPerfil(UsuarioViewModel usuarioModif)
    {
        if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString())
        {
            return RedirectToAction("Index", "Home");
        }
        if (ModelState.IsValid)
        {
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
                _logger.LogWarning(ex.ToString());
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error");
            }
            catch (SqliteException ex)
            {
                _logger.LogError($"Error en base de datos en EditPerfil: {ex.ToString()}");
                ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en EditPerfil: {ex.ToString()}");
                ViewData["ErrorMessage"] = "Hubo un problema al actualizar el perfil del usuario.";
                return View("Error");
            }
        }

        return View(usuarioModif);
    }
    [HttpGet]
    [VerificarSesion]
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
            _logger.LogError($"Error en base de datos en UpdatePass: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en UpdatePass: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al cargar el perfil del usuario.";
            return View("Error");
        }
    }
    [HttpPost]
    [VerificarSesion]
    public IActionResult UpdatePass(UsuarioPasswordViewModel passModif)
    {
        if(HttpContext.Session.GetInt32("idUsuario").Value != passModif.IdUsuario)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View(passModif);
        }
        try
        {
            int idUsuario = (int)HttpContext.Session.GetInt32("idUsuario");
            var usuarioExistente = _usuarioRepository.GetById(idUsuario);
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
                Id_usuario = idUsuario,
                Nombre_de_usuario = usuarioExistente.Nombre_de_usuario,
                Password = passModif.NewPassword,
                Rol_usuario = usuarioExistente.Rol_usuario
            };
            _usuarioRepository.UpdatePass(usuarioModif, idUsuario);
            return RedirectToAction("Index", "Home");
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en UpdatePass: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View(passModif);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en UpdatePass: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al actualizar el perfil del usuario.";
            return View(passModif);  // No redirigir, solo retornar la vista
        }
    }






    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
