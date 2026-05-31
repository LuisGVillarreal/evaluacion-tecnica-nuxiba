namespace NuxibaAPI.DTOs;

// DTO para crear un nuevo registro de login/logout
public class LoginCreateDto
{
    public int User_id { get; set; }
    public int Extension { get; set; }
    public int TipoMov { get; set; }
    public DateTime Fecha { get; set; }
}
