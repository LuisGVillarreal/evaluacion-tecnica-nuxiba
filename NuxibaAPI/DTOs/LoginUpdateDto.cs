namespace NuxibaAPI.DTOs;

// DTO para actualizar un registro de login/logout
public class LoginUpdateDto
{
    public int User_id { get; set; }
    public int Extension { get; set; }
    public int TipoMov { get; set; }
    public DateTime Fecha { get; set; }
}
