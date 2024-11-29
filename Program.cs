using ITableroRepo;
using ITareaRepo;
using IUsuarioRepo;
using TableroRepo;
using TareaRepo;
using UsuarioRepo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var CadenaDeConexion = builder.Configuration.GetConnectionString("SqliteConexion")!.ToString();
builder.Services.AddSingleton<string>(CadenaDeConexion);

builder.Services.AddScoped<ITableroRepository,TableroRepository>();
builder.Services.AddScoped<ITareaRepository,TareaRepository>();
builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();


// Config de Session y Cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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
