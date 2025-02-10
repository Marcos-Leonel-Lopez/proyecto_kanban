using IColorRepo;
using ITableroRepo;
using ITareaRepo;
using IUsuarioRepo;
using TableroRepo;
using TareaRepo;
using UsuarioRepo;
using ColorRepo;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Config conexión a BD
var CadenaDeConexion = builder.Configuration.GetConnectionString("SqliteConexion")!.ToString();
builder.Services.AddSingleton<string>(CadenaDeConexion);

// Configuración de repositorios
builder.Services.AddScoped<ITableroRepository,TableroRepository>();
builder.Services.AddScoped<ITareaRepository,TareaRepository>();
builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();
builder.Services.AddScoped<IColorRepository,ColorRepository>();

builder.Services.AddScoped<ExceptionHandlerService>();

// Config de Serilog a partir de appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

// Reemplazar el sistema de logs de ASP.NET Core con Serilog
builder.Host.UseSerilog();

// Config de Session y Cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("Session:IdleTimeout"));
    options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("CookieHttpOnly");
    options.Cookie.IsEssential = builder.Configuration.GetValue<bool>("CookieIsEssential");
});

// Config. Bcrypt
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

int workFactor = configuration.GetValue<int>("BCrypt:WorkFactor");
BCryptService.SetWorkFactor(workFactor);


var app = builder.Build();

app.UseSerilogRequestLogging(); // Registra automáticamente logs de cada solicitud HTTP

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
