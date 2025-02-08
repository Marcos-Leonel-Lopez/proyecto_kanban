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

var CadenaDeConexion = builder.Configuration.GetConnectionString("SqliteConexion")!.ToString();
builder.Services.AddSingleton<string>(CadenaDeConexion);

builder.Services.AddScoped<ITableroRepository,TableroRepository>();
builder.Services.AddScoped<ITareaRepository,TareaRepository>();
builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();
builder.Services.AddScoped<IColorRepository,ColorRepository>();

// Config de Serilog a partir de appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(builder.Configuration)
//     .WriteTo.File(
//         "Logs/logs.txt",        // Ruta donde se guardarán los logs
//         rollingInterval: RollingInterval.Month, // Crea un archivo nuevo por día
//         outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", // Formato de logs
//         restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information // Nivel mínimo de logs que se guardarán
//         // restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug // Guarda TODOS los logs
//     )
//     .CreateLogger();


builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

// Reemplazar el sistema de logs de ASP.NET Core con Serilog
builder.Host.UseSerilog();

// Config de Session y Cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
