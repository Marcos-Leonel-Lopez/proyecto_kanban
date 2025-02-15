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
    private readonly ExceptionHandlerService _exceptionHandler;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    public TareaController(ILogger<TareaController> logger, ExceptionHandlerService exceptionHandler, ITareaRepository tareaRepository, ITableroRepository tableroRepository, IColorRepository colorRepository, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _exceptionHandler = exceptionHandler;
        _tareaRepository = tareaRepository;
        _tableroRepository = tableroRepository;
        _colorRepository = colorRepository;
        _usuarioRepository = usuarioRepository;
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Index()
    {
        var rol = HttpContext.Session.GetString("rolUsuario");
        ViewData["rolUsuario"] = rol;
        if (rol == MisEnums.RolUsuario.Administrador.ToString())
        {
            List<UsuarioViewModel> usaurios = _usuarioRepository.GetAll()
            .Select(u => new UsuarioViewModel
            {
                Id_usuario = u.Id_usuario,
                Nombre = u.Nombre_de_usuario
            })
            .ToList();
            ViewData["Usuarios"] = usaurios;
        }
        return View();
    }
    // [HttpGet]
    // [AccessAuthorize("Administrador", "Operador")]
    // public IActionResult GetByTablero(int idTablero)
    // {
    //     try
    //     {
    //         Tablero tablero = _tableroRepository.GetById(idTablero);
    //         ListarTablerosUsuarioViewModel viewModel = new ListarTablerosUsuarioViewModel
    //         {
    //             Id_tablero = tablero.Id_tablero,
    //             Nombre = tablero.Nombre,
    //             Tareas = _tareaRepository.GetByTablero(idTablero)
    //             .Select(tarea => new TareaListaViewModel
    //             {
    //                 Nombre = tarea.Nombre,
    //                 Descripcion = tarea.Descripcion,
    //                 Estado = tarea.Id_estado.ToString()
    //             }).ToList()
    //         };
    //         return View(viewModel);
    //     }
    //     catch (Exception ex)
    //     {
    //         return _exceptionHandler.HandleException(ex, "Tarea", nameof(GetByTablero));
    //     }
    // }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Create()
    {
        int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
        var tableros = _tableroRepository.GetAll()
            .Where(tablero => tablero.Id_usuario_propietario == idUsuarioLogueado)
            .Select(tablero => new TableroViewModel(tablero, _usuarioRepository.GetById(tablero.Id_usuario_propietario).Nombre_de_usuario))
            .ToList();

        var colores = _colorRepository.GetAll();
        var usuarios = _usuarioRepository.GetAll()
            .Select(usuario => new UsuarioViewModel(usuario))
            .ToList();

        var tareasCreadas = _tareaRepository.GetAll()
        .Where(t => tableros.Any(tab => tab.Id_tablero == t.Id_tablero)) // Solo tareas de tableros del usuario
        .GroupBy(t => t.Id_tablero)
        .Select(g => new TareasCreadasViewModel
        {
            Nombre_tablero = tableros.First(tab => tab.Id_tablero == g.Key).Nombre,
            Tareas = g.Select(t => new TareaListaViewModel
            {
                Nombre = t.Nombre,
            }).ToList()
        }).ToList();

        var nuevaTarea = new CrearTareaViewModel
        {
            NuevaTarea = new NuevaTareaViewModel(),
            Tableros = tableros,
            Colores = colores,
            Usuarios = usuarios,
            TareasCreadas = tareasCreadas
        };
        return View(nuevaTarea);
    }
    [HttpPost]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Create(CrearTareaViewModel model, string returnUrl = null)
    {
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

            // Redirigir a la URL de origen si está definida, sino a Create
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tarea", nameof(Create));
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult ActualizarEstado(KanbanViewModel model)
    {
        try
        {
            int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
            if (idUsuarioLogueado != model.TareaModificada.Id_usuario_asignado)
            {
                string nombre = HttpContext.Session.GetString("nombreUsuario");
                throw new NoAutorizadoException(nombre, idUsuarioLogueado, HttpContext.Request.Path);
            }
            var tareaAnterior = _tareaRepository.GetById(model.TareaModificada.Id_tarea);
            var nuevoEstado = (int)model.TareaModificada.Id_estado;
            _tareaRepository.UpdateEstado(tareaAnterior, nuevoEstado);
            return RedirectToAction("Kanban", "Tablero", new { id_tablero = tareaAnterior.Id_tablero });
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tarea", nameof(ActualizarEstado));
        }
    }
    [HttpPost]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult ActualizarUsuario(KanbanViewModel model)
    {
        // verificar que el creador del tablero y el id de quien esta logueado sean iguales
        try
        {
            int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
            if (idUsuarioLogueado != model.IdPropietario)
            {
                string nombre = HttpContext.Session.GetString("nombreUsuario");
                throw new NoAutorizadoException(nombre, idUsuarioLogueado, HttpContext.Request.Path);
            }
            var tareaAnterior = _tareaRepository.GetById(model.TareaModificada.Id_tarea);
            int? nuevoUsuario = model.TareaModificada.Id_usuario_asignado;
            _tareaRepository.UpdateUsuario(tareaAnterior, nuevoUsuario);
            return RedirectToAction("Kanban", "Tablero", new { id_tablero = tareaAnterior.Id_tablero });
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tarea", nameof(ActualizarUsuario));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult GetByUsuario(int idUsuario)
    {
        try
        {
            if (HttpContext.Session.GetString("rolUsuario") != MisEnums.RolUsuario.Administrador.ToString() && HttpContext.Session.GetInt32("idUsuario").Value != idUsuario)
            {
                int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
                string nombre = HttpContext.Session.GetString("nombreUsuario");
                throw new NoAutorizadoException(nombre, idUsuarioLogueado, HttpContext.Request.Path);
            }
            _usuarioRepository.GetById(idUsuario);
            List<ListarTablerosUsuarioViewModel> viewModel = new List<ListarTablerosUsuarioViewModel>();
            // Obtener solo las tareas donde el usuario está asignado
            var tareasUsuario = _tareaRepository.GetByUsuario(idUsuario);

            // Agrupar las tareas por tablero
            var tareasAgrupadasPorTablero = tareasUsuario
                .GroupBy(t => t.Id_tablero)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var diccionario in tareasAgrupadasPorTablero)
            {
                int idTablero = diccionario.Key;
                var tareas = diccionario.Value;

                var tablero = _tableroRepository.GetById(idTablero);

                viewModel.Add(new ListarTablerosUsuarioViewModel
                {
                    Id_tablero = tablero.Id_tablero,
                    Nombre = tablero.Nombre,
                    Tareas = tareas.Select(tarea => new TareaListaViewModel
                    {
                        Nombre = tarea.Nombre,
                        Descripcion = tarea.Descripcion,
                        Estado = tarea.Id_estado.ToString()
                    }).ToList()
                });
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tarea", nameof(GetByUsuario));
        }
    }
    [HttpGet]
    [AccessAuthorize("Administrador", "Operador")]
    public IActionResult Delete(int idTarea)
    {
        try
        {
            Tarea tarea = _tareaRepository.GetById(idTarea);
            Tablero tablero = _tableroRepository.GetById(tarea.Id_tablero);
            int idUsuarioLogueado = HttpContext.Session.GetInt32("idUsuario").Value;
            if (idUsuarioLogueado != tablero.Id_usuario_propietario)
            {
                string nombre = HttpContext.Session.GetString("nombreUsuario");
                throw new NoAutorizadoException(nombre, idUsuarioLogueado, HttpContext.Request.Path);
            }
            _tareaRepository.Remove(idTarea);
            return RedirectToAction("Kanban", "Tablero", new { id_tablero = tablero.Id_tablero });
        }
        catch (Exception ex)
        {
            return _exceptionHandler.HandleException(ex, "Tarea", nameof(Delete));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}