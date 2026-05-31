using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAPI.Models;

[Table("ccloglogin")]
public class Login
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int User_id { get; set; }

    [Required]
    public int Extension { get; set; }

    /// <summary>
    /// 1 = Login, 0 = Logout
    /// </summary>
    [Required]
    [Range(0, 1, ErrorMessage = "TipoMov debe ser 0 (logout) o 1 (login).")]
    public int TipoMov { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; }

    // Relación con la tabla Users
    [ForeignKey("User_id")]
    public User? User { get; set; }
}
