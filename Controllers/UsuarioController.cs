using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using IUsuarioRepo;

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
    public IActionResult Index() // Igual al nombre de los archivos en carpeta "Views"
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var usuarios = _usuarioRepository.GetAll();
            var viewModel = usuarios.Select(usuario => new UsuarioViewModel
            {
                Id = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            }).ToList();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetAll: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener los usuarios.";
            return View("Error");
        }
    }
    [HttpGet]
    public IActionResult GetById(int idUsuario)
    {
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
                Id = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetById: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener el usuario.";
            return View("Error");
        }
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Usuario());
    }

    [HttpPost]
    public IActionResult Create(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _usuarioRepository.Create(usuario);
                return RedirectToAction("GetAll");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Create: {ex.Message}");
                ViewData["ErrorMessage"] = "Hubo un problema al crear el usuario.";
                return View("Error");
            }
        }
        return View(usuario);
    }
    [HttpGet]
    // solo podria acceder un admin
    public IActionResult ListToEdit()
    {
        try
        {
            var usuarios = _usuarioRepository.GetAll();
            var viewModel = usuarios.Select(usuario => new UsuarioViewModel
            {
                Id = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            }).ToList();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en ListToEdit: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener la lista de usuarios para editar.";
            return View("Error");
        }
    }
    [HttpGet]
    public IActionResult Delete(int idUsuario)
    {
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
                Id = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Delete: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener el usuario para eliminar.";
            return View("Error");
        }
    }
    [HttpPost]
    public IActionResult DeleteConfirmed(int idUsuario)
    {
        try
        {
            var usuario = _usuarioRepository.GetById(idUsuario);
            if (usuario == null)
            {
                ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
                return View("Error");
            }
            _usuarioRepository.Remove(idUsuario);
            return RedirectToAction("GetAll");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en DeleteConfirmed: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al eliminar el usuario.";
            return View("Error");
        }
    }
    [HttpGet]
    // Verificar que el usuario sea admin o que el id de las ession coincida con el 'idUsuario'
    public IActionResult EditarPerfil(int idUsuario)
    {
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
                Id = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en EditarPerfil: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al cargar el perfil del usuario.";
            return View("Error");
        }
    }
    [HttpPost]
    public IActionResult EditPerfil(UsuarioViewModel usuarioModif)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var usuarioExistente = _usuarioRepository.GetById(usuarioModif.Id);
                if (usuarioExistente == null)
                {
                    ViewData["ErrorMessage"] = "Hubo un problema al intentar actualizar el usuario.";
                    return View("Error");
                }
                var modif = _usuarioRepository.EditarPerfil(usuarioModif, usuarioModif.Id);
                if (!modif)
                {
                    ViewData["ErrorMessage"] = "Hubo un problema al intentar actualizar el usuario.";
                    return View("Error");
                }
                return RedirectToAction("GetAll");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en EditPerfil: {ex.Message}");
                ViewData["ErrorMessage"] = "Hubo un problema al actualizar el perfil del usuario.";
                return View("Error");
            }
        }

        return View(usuarioModif);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
