using System.ComponentModel.DataAnnotations;

namespace OSControlSystem.Models
{
    public class ListaDistribucion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Emails { get; set; } // Lista separada por comas o semicolons
    }
}
