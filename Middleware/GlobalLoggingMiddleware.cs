using System.Diagnostics;
using Serilog.Context; // Necesario para inyectar el CorrelationId en Serilog

namespace VulnerableApp.Middlewares
{
    public class GlobalLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalLoggingMiddleware> _logger;

        public GlobalLoggingMiddleware(RequestDelegate next, ILogger<GlobalLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Iniciamos el cronómetro (Request Logging)
            var stopwatch = Stopwatch.StartNew();

            // Generamos el ID único (CorrelationId)
            var correlationId = Guid.NewGuid().ToString();

            // 1. Agregar a los headers de respuesta para el cliente
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            // Inyectamos el CorrelationId en Serilog para que TODOS los logs de esta petición lo tengan
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    // Pasamos la solicitud al resto de la aplicación (controladores, vistas, etc.)
                    await _next(context);

                    // 2. Request Logging: Registro exitoso al terminar
                    stopwatch.Stop();
                    _logger.LogInformation(
                        "HTTP {RequestMethod} {RequestPath} respondió con {StatusCode} en {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    // 3. Exception Middleware: Atrapamos cualquier error que no se haya manejado
                    stopwatch.Stop();

                    _logger.LogError(ex, "Unhandled Exception: Error crítico interceptado por el middleware. Ruta: {RequestPath}. Tiempo: {ElapsedMilliseconds} ms", context.Request.Path, stopwatch.ElapsedMilliseconds);

                    // Retornamos un error 500 limpio al usuario
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Ocurrió un error interno en el servidor. Por favor, intente más tarde.");
                }
            }
        }
    }
}