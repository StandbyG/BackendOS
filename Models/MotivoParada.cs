using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSControlSystem.Models
{
    public class MotivoParada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Tipo { get; set; } // Planificada o No Planificada

        [ForeignKey("ListaDistribucion")]
        public int ListaDistribucionId { get; set; }
        public ListaDistribucion ListaDistribucion { get; set; }
    }
}
