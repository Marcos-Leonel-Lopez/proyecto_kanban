using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado

using IUsuarioRepo;

namespace trabajo_final.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }
    [HttpGet]
    public IActionResult Index() // Igual al nombre de los archivos en carpeta "Views"
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var usuarios = _usuarioRepository.GetAll();
        var viewModel = usuarios.Select(usuario => new DataUsuario(usuario)).ToList();
        return View(viewModel);
    }
    [HttpGet]
    public IActionResult GetById(int idUsuario)
    {
        var viewModel = new DataUsuario(_usuarioRepository.GetById(idUsuario));
        if (viewModel == null)
        {
            ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
            return View("Error");
        }
        return View(viewModel);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Usuario());
    }
    [HttpPost]
    public IActionResult Create(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            _usuarioRepository.Create(usuario);
            return RedirectToAction("GetAll");
        }
        return View(usuario);
    }
    [HttpGet]
    // solo podria acceder un admin
    public IActionResult ListToEdit()
    {
        var usuarios = _usuarioRepository.GetAll();
        var viewModel = usuarios.Select(usuario => new DataUsuario(usuario)).ToList();
        return View(viewModel);
    }
    [HttpGet]
    public IActionResult Delete(int idUsuario)
    {
        var usuario = _usuarioRepository.GetById(idUsuario);
        if (usuario == null)
        {
            ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
            return View("Error");
        }
        var viewModel = new DataUsuario(usuario);
        return View(viewModel);
    }
    [HttpPost]
    public IActionResult DeleteConfirmed(int idUsuario)
    {
        var usuario = _usuarioRepository.GetById(idUsuario);
        if (usuario == null)
        {
            ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
            return View("Error");
        }
        _usuarioRepository.Remove(idUsuario);
        return RedirectToAction("GetAll");
    }
    [HttpGet]
    public IActionResult Update(int idUsuario){
        var usuario = _usuarioRepository.GetById(idUsuario);
        if(usuario == null){
            ViewData["ErrorMessage"] = "El usuario con el ID proporcionado no existe.";
            return View("Error");
        }
        return View(usuario);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}