using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Data;
using System.Text;

namespace NuxibaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/reports/csv
    [HttpGet("csv")]
    public async Task<IActionResult> DownloadCsv()
    {
        try
        {
            // Se obtienen todos los usuarios con su area y sus logins
            var usuarios = await _context.Users
                .Include(u => u.Area)
                .Include(u => u.Logins)
                .ToListAsync();

            var sb = new StringBuilder();

            // Se escribe la fila de encabezados
            sb.AppendLine("NombreUsuario,NombreCompleto,Area,TotalHorasTrabajadas");

            foreach (var usuario in usuarios)
            {
                // Se obtienen los logins ordenados por fecha
                var logins = usuario.Logins
                    .Where(l => l.TipoMov == 1)
                    .OrderBy(l => l.Fecha)
                    .ToList();

                var logouts = usuario.Logins
                    .Where(l => l.TipoMov == 0)
                    .OrderBy(l => l.Fecha)
                    .ToList();

                // Se emparejan logins con logouts y se suma el tiempo
                double totalHoras = 0;
                int pares = Math.Min(logins.Count, logouts.Count);

                for (int i = 0; i < pares; i++)
                {
                    var diferencia = logouts[i].Fecha - logins[i].Fecha;
                    totalHoras += diferencia.TotalHours;
                }

                // Se construye la fila del CSV
                var nombreCompleto = $"{usuario.Nombres} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno}";
                var area = usuario.Area?.AreaName ?? "Sin area";

                sb.AppendLine($"{usuario.Login},{nombreCompleto},{area},{totalHoras:F2}");
            }

            // Se convierte a bytes y se retorna como archivo descargable
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "reporte_horas.csv");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al generar el CSV.", error = ex.Message });
        }
    }
}
