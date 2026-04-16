using Aplicacion.Abstracciones;
using Aplicacion.Inyecciones;
using Infraestructura.Data;
using Infraestructura.Repositorios;
using Microsoft.EntityFrameworkCore;
using Presentacion.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar DbContext
builder.Services.AddDbContext<ContextoECE>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBD")));
// Registra UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AgregarAplicacion();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();