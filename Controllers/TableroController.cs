using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using IUsuarioRepo;
using ITableroRepo;
using ITareaRepo;
using IColorRepo;
// using System.Security.Cryptography.X509Certificates;

namespace trabajo_final.Controllers;

public class TableroController : Controller
{
    private readonly ILogger<TableroController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITableroRepository _tableroRepository;
    private readonly ITareaRepository _tareasRepository;
    private readonly IColorRepository _colorRepository;
    public TableroController(ILogger<TableroController> logger, IUsuarioRepository usuarioRepository, ITableroRepository tableroRepository, ITareaRepository tareasRepository, IColorRepository colorRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
        _tableroRepository = tableroRepository;
        _tareasRepository = tareasRepository;
        _colorRepository = colorRepository;
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
        var nuevoTablero = new CrearTableroViewModel
        {
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

    public IActionResult Kanban(int id_tablero)
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            // Obtener los colores disponibles.
            var colores = _colorRepository.GetAll();

            // Mapear las tareas a TareaEnTableroViewModel.
            var tareasViewModel = _tareasRepository.GetByTablero(id_tablero)
                .Select(tarea => new TareaEnTableroViewModel
                {
                    Id_tarea = tarea.Id_tarea,
                    Nombre = tarea.Nombre,
                    Descripcion = tarea.Descripcion,
                    Id_estado = tarea.Id_estado,
                    Id_usuario_asignado = tarea.Id_usuario_asignado,
                    Codigo_color = colores.FirstOrDefault(c => c.Id_color == tarea.Id_color)?.Hex ?? "#FFFFFF"
                }).ToList();

            // Crear el ViewModel del Kanban.
            var kanbanViewModel = new KanbanViewModel
            {
                Tareas = tareasViewModel,
                TareaModificada = new TareaEnTableroViewModel() // Inicializamos una tarea vacía.
            };

            // Pasar el ID del usuario logueado a la vista.
            ViewData["idUsuarioLogueado"] = (int)HttpContext.Session.GetInt32("idUsuario");

            // Devolver la vista con el ViewModel.
            return View(kanbanViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Kanban: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al mostrar el tablero.";
            return View("Error");
        }
    }








    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}