using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado
using IUsuarioRepo;
using ITableroRepo;

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
        return View();
    }





    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}