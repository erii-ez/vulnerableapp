using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using VulnerableApp.Models;
using System.Diagnostics;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger; 

        public SearchController(AppDbContext db, ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            var stopwatch = Stopwatch.StartNew();

            var user = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";

            _logger.LogInformation("Inicio Search.Index");
            _logger.LogInformation("Usuario: {User} IP: {IP} Parámetro de búsqueda: {SearchParam}", user, ip, search);

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    _logger.LogWarning("El usuario {User} intentó realizar una búsqueda sin términos", user);

                    stopwatch.Stop();
                    _logger.LogInformation("Fin Search.Index. Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                    return View(new List<User>());
                }

                var users = _db.Users
                    .Where(u => u.Username.Contains(search))
                    .ToList();

                stopwatch.Stop();
                _logger.LogInformation("Fin Search.Index. Tiempo de ejecución: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                return View(users);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error crítico durante la búsqueda en Search.Index. Tiempo antes del fallo: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}