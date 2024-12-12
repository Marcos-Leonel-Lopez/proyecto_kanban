using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using trabajo_final.Models;
using Microsoft.AspNetCore.Session; //Agregado

using IUsuarioRepo;

namespace trabajo_final.Controllers;

public class UsuarioController : Controller{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository){
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }
    [HttpGet]
    public IActionResult Index() // Igual al nombre de los archivos en carpeta "Views"
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetAll(){
        var usuarios = _usuarioRepository.GetAll();
        var viewModel = usuarios.Select(usuario => new DataUsuario(usuario)).ToList();
        return View(viewModel);
    }
    
}