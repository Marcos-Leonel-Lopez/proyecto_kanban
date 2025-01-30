using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using ITareaRepo;
using ITableroRepo;
using IColorRepo;
using IUsuarioRepo;
using Microsoft.Data.Sqlite;

namespace trabajo_final.Controllers;

public class TareaController : Controller
{
    private readonly ILogger<TareaController> _logger;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    public TareaController(ILogger<TareaController> logger, ITareaRepository tareaRepository, ITableroRepository tableroRepository, IColorRepository colorRepository, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _tareaRepository = tareaRepository;
        _tableroRepository = tableroRepository;
        _colorRepository = colorRepository;
        _usuarioRepository = usuarioRepository;
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
    public IActionResult GetByTablero(int idTablero)
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            List<Tarea> tareas = _tareaRepository.GetByTablero(idTablero);
            return View(tareas);
        }
        catch (SqliteException ex){
            _logger.LogError($"Error en base de datos en GetByTablero: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetByTablero: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener las tareas.";
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
        var tableros = _tableroRepository.GetAll()
            .Where(tablero => tablero.Id_usuario_propietario == HttpContext.Session.GetInt32("idUsuario").Value)
            .Select(tablero => new TableroViewModel(tablero, _usuarioRepository.GetById(tablero.Id_usuario_propietario).Nombre_de_usuario))
            .ToList();

        var colores = _colorRepository.GetAll();
        var usuarios = _usuarioRepository.GetAll()
            .Select(usuario => new UsuarioViewModel(usuario))
            .ToList();
        var nuevaTarea = new CrearTareaViewModel(new NuevaTareaViewModel(), tableros, colores, usuarios);
        return View(nuevaTarea);
    }
    [HttpPost]
    public IActionResult Create(CrearTareaViewModel model)
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        if (!ModelState.IsValid)
        {
            // Si hay errores de validación, recarga la vista con los datos actuales
            model.Tableros = _tableroRepository.GetAll()
            .Select(tablero => new TableroViewModel(tablero, _usuarioRepository.GetById(tablero.Id_usuario_propietario).Nombre_de_usuario))
            .ToList(); // Vuelve a cargar los datos necesarios
            model.Colores = _colorRepository.GetAll();
            model.Usuarios = _usuarioRepository.GetAll()
            .Select(usuario => new UsuarioViewModel(usuario))
            .ToList();
            return View(model);
        }

        try
        {
            var nuevaTarea = new Tarea
            {
                Nombre = model.NuevaTarea.Nombre,
                Descripcion = model.NuevaTarea.Descripcion,
                Id_color = model.NuevaTarea.Id_color,
                Id_estado = MisEnums.EstadoTarea.Ideas, //por defecto las tareas se crean "ideas"
                Id_usuario_asignado = model.NuevaTarea.Id_usuario_asignado
            };
            _tareaRepository.Create(nuevaTarea, model.NuevaTarea.Id_tablero);
            return RedirectToAction("Create");
        }
        catch (SqliteException ex){
            _logger.LogError($"Error en base de datos en Create: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Create: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al crear la tarea.";
            return View("Error");
        }

    }
    [HttpPost]
    public IActionResult ActualizarEstado(KanbanViewModel model)
    {
        if (HttpContext.Session.GetString("idUsuario") == null
            || HttpContext.Session.GetInt32("idUsuario") != model.TareaModificada.Id_usuario_asignado
            )
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var tareaAnterior = _tareaRepository.GetById(model.TareaModificada.Id_tarea);
            var nuevoEstado = (int)model.TareaModificada.Id_estado;
            _tareaRepository.UpdateEstado(tareaAnterior, nuevoEstado);
            return RedirectToAction("Kanban", "Tablero", new { id_tablero = tareaAnterior.Id_tablero });
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex){
            _logger.LogError($"Error en base de datos en ActualizarEstado: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Kanban: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al actualizar tarea.";
            return View("Error");
        }
    }
        [HttpPost]
    public IActionResult ActualizarUsuario(KanbanViewModel model)
    {
        if (HttpContext.Session.GetString("idUsuario") == null)
        {
            return RedirectToAction("Index", "Home");
        }
        try
        {
            var tareaAnterior = _tareaRepository.GetById(model.TareaModificada.Id_tarea);
            int? nuevoUsuario = model.TareaModificada.Id_usuario_asignado;
            _tareaRepository.UpdateUsuario(tareaAnterior, nuevoUsuario);
            return RedirectToAction("Kanban", "Tablero", new { id_tablero = tareaAnterior.Id_tablero });
        }
        catch (NoEncontradoException ex)
        {
            _logger.LogWarning(ex.ToString());
            ViewData["ErrorMessage"] = ex.Message;
            return View("Error");
        }
        catch (SqliteException ex){
            _logger.LogError($"Error en base de datos en ActualizarUsuario: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema con la base de datos. Intente más tarde.";
            return View("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en Kanban: {ex.ToString()}");
            ViewData["ErrorMessage"] = "Hubo un problema al actualizar tarea.";
            return View("Error");
        }
    }


}