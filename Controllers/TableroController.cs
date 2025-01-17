using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using IUsuarioRepo;
using ITableroRepo;
// using System.Security.Cryptography.X509Certificates;

namespace trabajo_final.Controllers;

public class TableroController : Controller
{
    private readonly ILogger<TableroController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITableroRepository _tableroRepository;
    public TableroController(ILogger<TableroController> logger, IUsuarioRepository usuarioRepository, ITableroRepository tableroRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
        _tableroRepository = tableroRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    [HttpGet]
    public IActionResult GetByUsuario()
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario").Value;
            List<TableroViewModel> tablerosPropio = _tableroRepository.GetByUsuario(idUsuario)
            .Select(tablero => new TableroViewModel
            {
                Id_tablero = tablero.Id_tablero,
                Nombre = tablero.Nombre,
                Descripcion = tablero.Descripcion,
                NombrePropietario = _usuarioRepository.GetById(tablero.Id_usuario_propietario).Nombre_de_usuario,
                Id_propietario = tablero.Id_usuario_propietario
            }).ToList();
            var tablerosParticipante = _tableroRepository.GetByParticipante(idUsuario)
            .Select(tablero => new TableroViewModel
            {
                Id_tablero = tablero.Id_tablero,
                Nombre = tablero.Nombre,
                Descripcion = tablero.Descripcion,
                NombrePropietario = _usuarioRepository.GetById(tablero.Id_usuario_propietario).Nombre_de_usuario,
                Id_propietario = tablero.Id_usuario_propietario
            }).ToList();
            var viewModel = new ParticipacionTablerosViewModel(tablerosPropio, tablerosParticipante);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetByUser: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener los tableros.";
            return View("Error");
        }
    }
    [HttpGet]
    public IActionResult GetAll()
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var tableros = _tableroRepository.GetAll();
            List<int> id_usuariosReq = tableros.Select(tablero => tablero.Id_usuario_propietario).ToList();
            List<Usuario> usuariosReq = new List<Usuario>();
            foreach (int id_usuario in id_usuariosReq)
            {
                usuariosReq.Add(_usuarioRepository.GetById(id_usuario));
            }
            List<TableroViewModel> viewModel = tableros.Select(tablero => new TableroViewModel
            {
                Id_tablero = tablero.Id_tablero,
                Nombre = tablero.Nombre,
                Descripcion = tablero.Descripcion,
                NombrePropietario = usuariosReq.FirstOrDefault(u => u.Id_usuario == tablero.Id_usuario_propietario).Nombre_de_usuario,
                Id_propietario = tablero.Id_usuario_propietario
            }).ToList();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetAll: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener los tableros.";
            return View("Error");
        }
    }
    [HttpGet]
    public IActionResult GetById(int idTablero)
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var tablero = _tableroRepository.GetById(idTablero);
            if (tablero == null)
            {
                ViewData["ErrorMessage"] = "El tablero con el ID proporcionado no existe.";
                return View("Error");
            }
            var viewModel = new TableroViewModel
            {
                Id_tablero = tablero.Id_tablero,
                Nombre = tablero.Nombre,
                Descripcion = tablero.Descripcion,
                NombrePropietario = _usuarioRepository.GetById(tablero.Id_usuario_propietario).Nombre_de_usuario,
                Id_propietario = tablero.Id_usuario_propietario
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetAll: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener los tableros.";
            return View("Error");

        }

    }
    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        var propietario = HttpContext.Session.GetInt32("idUsuario").Value;
        var nuevoTablero = new CrearTableroViewModel{
            Id_usuario_propietario = HttpContext.Session.GetInt32("idUsuario").Value
        };
        return View(nuevoTablero);
    }
    [HttpPost]
    public IActionResult Create(CrearTableroViewModel nuevoTablero)
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        if (!ModelState.IsValid)
        {
            return View(nuevoTablero);
        }
            try
            {
                var tablero = new Tablero
                {
                    Id_usuario_propietario = HttpContext.Session.GetInt32("idUsuario").Value,
                    Nombre = nuevoTablero.Nombre,
                    Descripcion = nuevoTablero.Descripcion
                };
                _tableroRepository.Create(tablero);
                return RedirectToAction("GetAll");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Create: {ex.Message}");
                ViewData["ErrorMessage"] = "Hubo un problema al crear el tablero.";
                return View("Error");
            }
        }
        






    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}