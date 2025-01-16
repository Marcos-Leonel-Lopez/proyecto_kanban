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
                Id = tablero.Id_tablero,
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
                Id = tablero.Id_tablero,
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






    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}