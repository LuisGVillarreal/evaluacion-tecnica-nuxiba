using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAPI.Models;

[Table("ccRIACat_Areas")]
public class Area
{
    [Key]
    public int IDArea { get; set; }

    [Required]
    [MaxLength(100)]
    public string AreaName { get; set; } = string.Empty;

    public int StatusArea { get; set; }

    public DateTime CreateDate { get; set; }

    // Navegación hacia la tabla User (JOIN)
    public ICollection<User> Users { get; set; } = new List<User>();
}
