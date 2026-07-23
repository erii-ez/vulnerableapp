using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio HomeController.Index | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en HomeController.Index");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin HomeController.Index | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        public IActionResult Privacy()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio HomeController.Privacy | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en HomeController.Privacy");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin HomeController.Privacy | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            var usuario = HttpContext.Session.GetString("User") ?? "Anónimo";

            _logger.LogInformation("Inicio HomeController.Error | Usuario: {Usuario} | IP: {IP}", usuario, ip);

            try
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en HomeController.Error");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin HomeController.Error | Tiempo de ejecución: {TiempoMs} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}