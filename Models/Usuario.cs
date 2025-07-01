using System.ComponentModel.DataAnnotations;

namespace OSControlSystem.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Rol { get; set; } // Administrador, Técnico, Administrativo

        [Required]
        public string PasswordHash { get; set; }
    }
}
