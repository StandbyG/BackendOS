using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSControlSystem.Models
{
    public class Etapa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("OrdenServicio")]
        public int OrdenServicioId { get; set; }
        public OrdenServicio OrdenServicio { get; set; }

        [Required]
        public string Tipo { get; set; } // Desarme o Armado

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string? InformeExternoUrl { get; set; }
        public ICollection<Parada>? Paradas { get; set; }
        public ICollection<Reprogramacion>? Reprogramaciones { get; set; }
    }
}
