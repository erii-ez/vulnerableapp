using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data; // Importante: Asegúrate de incluir el namespace donde creaste tu AppDbContext

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// --- FIN DE ACTIVIDAD 5 ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseSession();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// La ruta del controlador por defecto que solicita el documento ya viene incluida en tu plantilla
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();