using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using ITareaRepo; 
using ITableroRepo;
using IColorRepo;

namespace trabajo_final.Controllers;

public class TareaController : Controller
{
    private readonly ILogger<TareaController> _logger;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository;
    private readonly IColorRepository _colorRepository;
    public TareaController(ILogger<TareaController> logger, ITareaRepository tareaRepository, ITableroRepository tableroRepository, IColorRepository colorRepository)
    {
        _logger = logger;
        _tareaRepository = tareaRepository;
        _tableroRepository = tableroRepository;
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
    public IActionResult GetByTablero(int idTablero)
    {
        try
        {
            List<Tarea> tareas = _tareaRepository.GetByTablero(idTablero);
            List<Color> colores = _colorRepository.GetAll();
            return View(tareas);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en GetByTablero: {ex.Message}");
            ViewData["ErrorMessage"] = "Hubo un problema al obtener las tareas.";
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Tarea());
    }
     [HttpPost]
    public IActionResult Create(Tarea tarea, int idTablero)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _tareaRepository.Create(tarea, idTablero);
                return RedirectToAction("GetByTablero", new { idTablero = idTablero });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Create: {ex.Message}");
                ViewData["ErrorMessage"] = "Hubo un problema al crear la tarea.";
                return View("Error");
            }
        }
        return View(tarea);
    }
    
}