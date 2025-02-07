using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using IUsuarioRepo;
using ITableroRepo;
using ITareaRepo;
using IColorRepo;
using Microsoft.Data.Sqlite;
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
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en GetByUser: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetByUser: {ex.ToString()}");
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
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en GetAll: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetAll: {ex.ToString()}");
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
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.ToString());
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
            return RedirectToAction("GetByUsuario");
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
            // Obtener el ID del usuario logueado.
            var idUsuarioLogueado = (int)HttpContext.Session.GetInt32("idUsuario");
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
            // Determinar si el usuario logueado es propietario del tablero.
            var esPropietario = _tableroRepository.GetById(id_tablero).Id_usuario_propietario == idUsuarioLogueado;
            // Obtengo los usuarios para asignar tareas.
            var usuarios = _usuarioRepository.GetAll()
                .Select(u => new UsuarioViewModel
                {
                    Id_usuario = u.Id_usuario,
                    Nombre = u.Nombre_de_usuario
                }).ToList();
            // Crear el ViewModel del Kanban.
            var kanbanViewModel = new KanbanViewModel
            {
                Tareas = tareasViewModel,
                TareaModificada = new TareaEnTableroViewModel(), // Inicializamos una tarea vacía.
                EsPropietario = esPropietario,
                Usuarios = usuarios
            };
            // Pasar el ID del usuario logueado a la vista.
            ViewData["idUsuarioLogueado"] = idUsuarioLogueado;
            // Devolver la vista con el ViewModel.
            return View(kanbanViewModel);
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en Kanban: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Kanban: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al mostrar el tablero.";
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult Remove(int id_tablero){
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            _tableroRepository.Remove(id_tablero);
            return RedirectToAction("GetByUsuario");
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex)
        {
            _logger.LogError($"Error en base de datos en Remove: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Remove: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al eliminar el tablero.";
            return View("Error");
        }
    }






    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}