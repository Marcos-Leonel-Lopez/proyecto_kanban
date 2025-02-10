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
    private readonly ExceptionHandlerService _exceptionHandler;
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioController(ILogger<UsuarioController> logger, ExceptionHandlerService exceptionHandler, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _exceptionHandler = exceptionHandler;
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
            var usuarios = _usuarioRepository.GetAll().Select(usuario => new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            }).ToList();
            
            return View(usuarios);
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(GetAll));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult GetById(int idUsuario)
    {
        try
        {
            if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString() && HttpContext.Session.GetInt32("idUsuario").Value != idUsuario)
            {
                int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
                string nombre = HttpContext.Session.GetString("nombreUsuario");
                throw new NoAutorizadoException(nombre, idUsuarioLogueado, HttpContext.Request.Path);
            }
            var usuario = _usuarioRepository.GetById(idUsuario);
            var viewModel = new UsuarioViewModel
            {
                Id_usuario = usuario.Id_usuario,
                Nombre = usuario.Nombre_de_usuario,
                Rol = usuario.Rol_usuario.ToString()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(GetById));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult Create()
    {
        return View(new Usuario());
    }

    [HttpPost]
    [AccessAuthorize("Administrador")]
    public IActionResult Create(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }
        try
        {
            _usuarioRepository.Create(usuario);
            return RedirectToAction("GetAll");
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(Create));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult ListToEdit()
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
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(ListToEdit));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    public IActionResult Delete(int idUsuario)
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
            ViewData["conTareas"] = _usuarioRepository.UserBusy(idUsuario);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(Delete));
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador")]
    public IActionResult DeleteConfirmed(int idUsuario)
    {
        try
        {
            var usuario = _usuarioRepository.GetById(idUsuario);
            _usuarioRepository.Remove(idUsuario);
            return RedirectToAction("ListToEdit");
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(DeleteConfirmed));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador")]
    // Verificar que el usuario sea admin o que el id de las ession coincida con el 'idUsuario'
    public IActionResult EditarPerfil(int idUsuario)
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
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(EditarPerfil));
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador")] //admin
    public IActionResult EditarPerfil(UsuarioViewModel usuarioModif)
    {
        if (!ModelState.IsValid)
        {
            return View(usuarioModif);
        }
        try
        {
            var usuarioExistente = _usuarioRepository.GetById(usuarioModif.Id_usuario);
            _usuarioRepository.EditarPerfil(usuarioModif, usuarioModif.Id_usuario);
            return RedirectToAction("ListToEdit");
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(EditarPerfil));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult UpdatePass()
    {
        try
        {
            int idUsuario = HttpContext.Session.GetInt32("idUsuario").Value;
            return View(new UsuarioPasswordViewModel
            {
                IdUsuario = idUsuario
            });
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(UpdatePass));
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
                throw new NoAutorizadoException(nombre, idUsuarioLogueado, HttpContext.Request.Path);
            }
            var usuarioExistente = _usuarioRepository.GetById(idUsuarioLogueado);
            if (!BCryptService.VerificarPassword(passModif.Password, usuarioExistente.Password))
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
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Usuario", nameof(UpdatePass));
        }
    }

    // public IActionResult scriptHash(){
    //     try
    //     {
    //         bool res = _usuarioRepository.scriptHash();
    //         return Ok(res);
    //     }
    //     catch (Exception ex)
    //     {
    //         return _exceptionHandler.HandleException(ex, "Usuario", nameof(UpdatePass));
    //     }
    // }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
