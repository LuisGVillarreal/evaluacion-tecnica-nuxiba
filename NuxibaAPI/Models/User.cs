using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAPI.Models;

[Table("ccUsers")]
public class User
{
    [Key]
    public int User_id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ApellidoMaterno { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Password { get; set; } = string.Empty;

    public int TipoUser_id { get; set; }

    public int Status { get; set; }

    public DateTime fCreate { get; set; }

    public int IDArea { get; set; }

    public DateTime? LastLoginAttempt { get; set; }

    // Relación con la tabla Area
    [ForeignKey("IDArea")]
    public Area? Area { get; set; }

    // Navegación hacia la tabla Login (JOIN)
    public ICollection<Login> Logins { get; set; } = new List<Login>();
}
