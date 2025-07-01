using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSControlSystem.Models
{
    public class Permiso
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public string Modulo { get; set; } // Ej: OrdenServicio, Parada, etc.

        [Required]
        public string Nivel { get; set; } // lectura, escritura, admin
    }
}
