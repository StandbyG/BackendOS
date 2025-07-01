using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSControlSystem.Models
{
    public class Parada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Etapa")]
        public int EtapaId { get; set; }
        public Etapa Etapa { get; set; }

        [Required]
        public string Tipo { get; set; } // Planificada o No Planificada

        [ForeignKey("MotivoParada")]
        public int MotivoId { get; set; }
        public MotivoParada Motivo { get; set; }

        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
    }
}
