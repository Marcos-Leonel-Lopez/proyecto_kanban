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
    private readonly ExceptionHandlerService _exceptionHandler;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITableroRepository _tableroRepository;
    private readonly ITareaRepository _tareasRepository;
    private readonly IColorRepository _colorRepository;
    public TableroController(ILogger<TableroController> logger, ExceptionHandlerService exceptionHandler, IUsuarioRepository usuarioRepository, ITableroRepository tableroRepository, ITareaRepository tareasRepository, IColorRepository colorRepository)
    {
        _logger = logger;
        _exceptionHandler = exceptionHandler;
        _usuarioRepository = usuarioRepository;
        _tableroRepository = tableroRepository;
        _tareasRepository = tareasRepository;
        _colorRepository = colorRepository;
    }

    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult GetByUsuario()
    {
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
            return _exceptionHandler.HandleException(ex, "Tablero", nameof(GetByUsuario));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult GetAll()
    {
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
            return _exceptionHandler.HandleException(ex, "Tablero", nameof(GetAll));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult GetById(int idTablero)
    {
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
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tablero", nameof(GetById));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Create()
    {
        var nuevoTablero = new CrearTableroViewModel
        {
            Id_usuario_propietario = HttpContext.Session.GetInt32("idUsuario").Value
        };
        return View(nuevoTablero);
    }
    [HttpPost]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Create(CrearTableroViewModel nuevoTablero)
    {
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
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tablero", nameof(Create));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Kanban(int id_tablero)
    {
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
            // Propietario
            var idPropietario = _tableroRepository.GetById(id_tablero).Id_usuario_propietario;
            // Determinar si el usuario logueado es propietario del tablero.
            var esPropietario = idPropietario == idUsuarioLogueado;
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
                Usuarios = usuarios,
                IdPropietario = idPropietario
            };
            // Pasar el ID del usuario logueado a la vista.
            ViewData["idUsuarioLogueado"] = idUsuarioLogueado;
            // Devolver la vista con el ViewModel.
            return View(kanbanViewModel);
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tablero", nameof(Kanban));
        }
    }

    [HttpPost]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Remove(int id_tablero)
    {
        try
        {
            _tableroRepository.Remove(id_tablero);
            return RedirectToAction("GetByUsuario");
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tablero", nameof(Remove));
        }
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}