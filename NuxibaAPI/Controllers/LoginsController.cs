using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Data;
using NuxibaAPI.DTOs;
using NuxibaAPI.Models;

namespace NuxibaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/logins
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Login>>> GetAll()
    {
        try
        {
            var logins = await _context.Logins
                .Include(l => l.User)
                .ToListAsync();

            return Ok(logins);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener los registros.", error = ex.Message });
        }
    }

    // POST /api/logins
    [HttpPost]
    public async Task<ActionResult<Login>> Create([FromBody] LoginCreateDto dto)
    {
        try
        {
            // Validar que el User_id exista en ccUsers
            var userExists = await _context.Users.AnyAsync(u => u.User_id == dto.User_id);
            if (!userExists)
                return BadRequest(new { message = $"El User_id {dto.User_id} no existe." });

            // Validar que la fecha no sea futura
            if (dto.Fecha > DateTime.Now)
                return BadRequest(new { message = "La fecha no puede ser futura." });

            // Validar que no se registre un login sin un logout anterior
            if (dto.TipoMov == 1)
            {
                var ultimoRegistro = await _context.Logins
                    .Where(l => l.User_id == dto.User_id)
                    .OrderByDescending(l => l.Fecha)
                    .FirstOrDefaultAsync();

                if (ultimoRegistro != null && ultimoRegistro.TipoMov == 1)
                    return BadRequest(new { message = "No se puede registrar un login sin un logout anterior." });
            }

            // Crear el registro
            var login = new Login
            {
                User_id = dto.User_id,
                Extension = dto.Extension,
                TipoMov = dto.TipoMov,
                Fecha = dto.Fecha
            };

            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = login.Id }, login);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear el registro.", error = ex.Message });
        }
    }

    // PUT /api/logins/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] LoginUpdateDto dto)
    {
        try
        {
            // Buscar el registro
            var login = await _context.Logins.FindAsync(id);
            if (login == null)
                return NotFound(new { message = $"Registro con Id {id} no encontrado." });

            // Validar que el User_id exista
            var userExists = await _context.Users.AnyAsync(u => u.User_id == dto.User_id);
            if (!userExists)
                return BadRequest(new { message = $"El User_id {dto.User_id} no existe." });

            // Validar que la fecha no sea futura
            if (dto.Fecha > DateTime.Now)
                return BadRequest(new { message = "La fecha no puede ser futura." });

            // Actualizar los campos
            login.User_id = dto.User_id;
            login.Extension = dto.Extension;
            login.TipoMov = dto.TipoMov;
            login.Fecha = dto.Fecha;

            await _context.SaveChangesAsync();

            return Ok(login);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar el registro.", error = ex.Message });
        }
    }

    // DELETE /api/logins/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Buscar el registro
            var login = await _context.Logins.FindAsync(id);
            if (login == null)
                return NotFound(new { message = $"Registro con Id {id} no encontrado." });

            // Eliminar
            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar el registro.", error = ex.Message });
        }
    }
}
